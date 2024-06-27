resource "aws_s3_bucket" "frontend" {
  bucket        = "${var.account_id}-${var.db_name}"
  force_destroy = true
}

resource "aws_s3_object" "frontend_default" {
  bucket       = aws_s3_bucket.frontend.id
  key          = "index.html"
  source       = "./DummyVersions/index.html"
  content_type = "text/html"

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

resource "aws_s3_bucket_ownership_controls" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  rule {
    object_ownership = "BucketOwnerEnforced"
  }
}

resource "aws_s3_bucket_policy" "frontend" {
  bucket = aws_s3_bucket.frontend.id

  policy = jsonencode({
    "Version" : "2008-10-17",
    "Id" : "PolicyForCloudFrontPrivateContent",
    "Statement" : [
      {
        "Sid" : "AllowCloudFrontServicePrincipal",
        "Effect" : "Allow",
        "Principal" : {
          "Service" : "cloudfront.amazonaws.com"
        },
        "Action" : "s3:GetObject",
        "Resource" : "${aws_s3_bucket.frontend.arn}/*",
        "Condition" : {
          "StringEquals" : {
            "AWS:SourceArn" : "${aws_cloudfront_distribution.frontend.arn}"
          }
        }
      }
    ]
  })
}