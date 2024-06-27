resource "aws_acm_certificate" "frontend" {
  domain_name       = var.domain_name
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }

  provider = aws.us-east-1
}

resource "aws_cloudfront_origin_access_control" "frontend" {
  name                              = "Frontend"
  description                       = "Allow cloudfront access to bucket with frontend html"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_response_headers_policy" "frontend" {
  name    = "security-policy"

  security_headers_config {
    frame_options {
      frame_option = "DENY"
      override = true
    }
    # content_type_options {
    #   override = true
    # }
    # referrer_policy {
    #   referrer_policy = "same-origin"
    #   override = true
    # }
    # xss_protection {
    #   mode_block = true
    #   protection = true
    #   override = true
    # }
    # strict_transport_security {
    #   access_control_max_age_sec = "63072000"
    #   include_subdomains = true
    #   preload = true
    #   override = true
    # }
    # content_security_policy {
    #   content_security_policy = "frame-ancestors 'none'; default-src 'self'; img-src 'self' data:; script-src 'self'; style-src 'self' 'unsafe-inline'; object-src 'none'; font-src 'self' https://fonts.gstatic.com/ https://fonts.googleapis.com/; connect-src 'self' http://localhost:5000 https://fonts.googleapis.com/ https://fonts.gstatic.com/ https://api.insurance.projects.bbdgrad.com/;"
    #   override = true
    # }
  }
}

resource "aws_cloudfront_distribution" "frontend" {
  origin {
    domain_name              = aws_s3_bucket.frontend.bucket_regional_domain_name
    origin_access_control_id = aws_cloudfront_origin_access_control.frontend.id
    origin_id                = "primaryS3"
  }

  enabled             = true
  is_ipv6_enabled     = false
  default_root_object = "index.html"

  aliases = [var.domain_name]

  default_cache_behavior {
    # Using the CachingDisabled managed policy ID:
    cache_policy_id        = "4135ea2d-6df8-44a3-9df3-4b5a84be39ad"
    allowed_methods        = ["GET", "HEAD", "OPTIONS"]
    cached_methods         = ["GET", "HEAD"]
    target_origin_id       = "primaryS3"
    viewer_protocol_policy = "https-only"

    response_headers_policy_id = aws_cloudfront_response_headers_policy.frontend.id
  }

  price_class = "PriceClass_200"

  restrictions {
    geo_restriction {
      restriction_type = "none"
      locations        = []
    }
  }

  viewer_certificate {
    acm_certificate_arn      = aws_acm_certificate.frontend.arn
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }

  custom_error_response {
    error_caching_min_ttl = 10
    error_code            = "403"
    response_code         = "200"
    response_page_path    = "/index.html"
  }
}