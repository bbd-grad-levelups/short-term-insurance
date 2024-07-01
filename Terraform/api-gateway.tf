locals {
  cors_allowed_origin = "*"
}

data "aws_lbs" "nlb" {}

data "aws_lb" "nlb" {
  arn = tolist(data.aws_lbs.nlb.arns)[0]
}

resource "aws_api_gateway_vpc_link" "nlb" {
  name        = "NLB"
  description = "VPC link to NLB"
  target_arns = [tolist(data.aws_lbs.nlb.arns)[0]]
}

resource "aws_acm_certificate" "backend_beanstalk" {
  domain_name       = "api.${var.domain_name}"
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_acm_certificate" "frontend_beanstalk" {
  domain_name       = "fe.${var.domain_name}"
  validation_method = "DNS"

  lifecycle {
    create_before_destroy = true
  }
}

##############################################################
# Frontend APIGW with cognito auth
##############################################################
resource "aws_api_gateway_domain_name" "frontend_api" {
  domain_name              = "fe.${var.domain_name}"
  regional_certificate_arn = aws_acm_certificate.frontend_beanstalk.arn

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_rest_api" "frontend_api" {
  name = "frontend"
  body = jsonencode({
    "openapi" : "3.0.1",
    "info" : {
      "title" : "frontend",
      "version" : "1.0"
    },
    "servers" : [{
      "url" : "https://fe.insurance.projects.bbdgrad.com",
      "x-amazon-apigateway-endpoint-configuration" : {
        "disableExecuteApiEndpoint" : true
      }
    }],
    "paths" : {
      "/" : {
        "get" : {
          "x-amazon-apigateway-integration" : {
            "connectionId" : aws_api_gateway_vpc_link.nlb.id,
            "httpMethod" : "GET",
            "uri" : "http://${data.aws_lb.nlb.dns_name}",
            "passthroughBehavior" : "when_no_match",
            "connectionType" : "VPC_LINK",
            "type" : "http_proxy"
          }
        },
        "options" : {
          "responses" : {
            "200" : {
              "description" : "200 response",
              "headers" : {
                "Access-Control-Allow-Origin" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Methods" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Headers" : {
                  "schema" : {
                    "type" : "string"
                  }
                }
              },
              "content" : {}
            }
          },
          "x-amazon-apigateway-integration" : {
            "type" : "mock",
            "responses" : {
              "default" : {
                "statusCode" : "200",
                "responseParameters" : {
                  "method.response.header.Access-Control-Allow-Methods" : "'GET,OPTIONS'",
                  "method.response.header.Access-Control-Allow-Headers" : "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'",
                  "method.response.header.Access-Control-Allow-Origin" : "'${local.cors_allowed_origin}'"
                }
              }
            },
            "requestTemplates" : {
              "application/json" : "{\"statusCode\": 200}"
            },
            "passthroughBehavior" : "when_no_templates"
          }
        }
      },
      "/{proxy+}" : {
        "x-amazon-apigateway-any-method" : {
          "parameters" : [{
            "name" : "proxy",
            "in" : "path",
            "required" : true,
            "schema" : {
              "type" : "string"
            }
          }],
          "security" : [{
            "cognito" : []
          }],
          "x-amazon-apigateway-integration" : {
            "connectionId" : aws_api_gateway_vpc_link.nlb.id,
            "httpMethod" : "ANY",
            "uri" : "http://${data.aws_lb.nlb.dns_name}/{proxy}",
            "passthroughBehavior" : "when_no_match",
            "connectionType" : "VPC_LINK",
            "type" : "http_proxy",
            "requestParameters" : {
              "integration.request.path.proxy" : "method.request.path.proxy"
            }
          }
        },
        "options" : {
          "parameters" : [{
            "name" : "proxy",
            "in" : "path",
            "required" : true,
            "schema" : {
              "type" : "string"
            }
          }],
          "responses" : {
            "200" : {
              "description" : "200 response",
              "headers" : {
                "Access-Control-Allow-Origin" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Methods" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Headers" : {
                  "schema" : {
                    "type" : "string"
                  }
                }
              },
              "content" : {}
            }
          },
          "x-amazon-apigateway-integration" : {
            "type" : "mock",
            "responses" : {
              "default" : {
                "statusCode" : "200",
                "responseParameters" : {
                  "method.response.header.Access-Control-Allow-Methods" : "'DELETE,GET,HEAD,OPTIONS,PATCH,POST,PUT'",
                  "method.response.header.Access-Control-Allow-Headers" : "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'",
                  "method.response.header.Access-Control-Allow-Origin" : "'${local.cors_allowed_origin}'"
                }
              }
            },
            "requestTemplates" : {
              "application/json" : "{\"statusCode\": 200}"
            },
            "passthroughBehavior" : "when_no_match"
          }
        }
      },
      "/swagger/{proxy+}" : {
        "get" : {
          "parameters" : [{
            "name" : "proxy",
            "in" : "path",
            "required" : true,
            "schema" : {
              "type" : "string"
            }
          }],
          "x-amazon-apigateway-integration" : {
            "type" : "http_proxy",
            "connectionId" : aws_api_gateway_vpc_link.nlb.id,
            "httpMethod" : "GET",
            "uri" : "http://${data.aws_lb.nlb.dns_name}/swagger/{proxy}",
            "responses" : {
              "default" : {
                "statusCode" : "200"
              }
            },
            "requestParameters" : {
              "integration.request.path.proxy" : "method.request.path.proxy"
            },
            "passthroughBehavior" : "when_no_match",
            "connectionType" : "VPC_LINK",
            "cacheNamespace" : "df9bas",
            "cacheKeyParameters" : ["method.request.path.proxy"]
          }
        },
        "options" : {
          "parameters" : [{
            "name" : "proxy",
            "in" : "path",
            "required" : true,
            "schema" : {
              "type" : "string"
            }
          }],
          "responses" : {
            "200" : {
              "description" : "200 response",
              "headers" : {
                "Access-Control-Allow-Origin" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Methods" : {
                  "schema" : {
                    "type" : "string"
                  }
                },
                "Access-Control-Allow-Headers" : {
                  "schema" : {
                    "type" : "string"
                  }
                }
              },
              "content" : {}
            }
          },
          "x-amazon-apigateway-integration" : {
            "type" : "mock",
            "responses" : {
              "default" : {
                "statusCode" : "200",
                "responseParameters" : {
                  "method.response.header.Access-Control-Allow-Methods" : "'GET,OPTIONS'",
                  "method.response.header.Access-Control-Allow-Headers" : "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'",
                  "method.response.header.Access-Control-Allow-Origin" : "'${local.cors_allowed_origin}'"
                }
              }
            },
            "requestTemplates" : {
              "application/json" : "{\"statusCode\": 200}"
            },
            "passthroughBehavior" : "when_no_match"
          }
        }
      }
    }
    "components" : {
      "securitySchemes" : {
        "cognito" : {
          "type" : "apiKey",
          "name" : "Authorization",
          "in" : "header",
          "x-amazon-apigateway-authtype" : "cognito_user_pools",
          "x-amazon-apigateway-authorizer" : {
            "providerARNs" : [aws_cognito_user_pool.prd_pool.arn],
            "type" : "cognito_user_pools"
          }
        }
      }
    },
    "x-amazon-apigateway-gateway-responses" : {
      "DEFAULT_4XX" : {
        "responseParameters" : {
          "gatewayresponse.header.Access-Control-Allow-Methods" : "'GET,OPTIONS'",
          "gatewayresponse.header.Access-Control-Allow-Origin" : "'${local.cors_allowed_origin}'",
          "gatewayresponse.header.Access-Control-Allow-Headers" : "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"
        }
      },
      "DEFAULT_5XX" : {
        "responseParameters" : {
          "gatewayresponse.header.Access-Control-Allow-Methods" : "'GET,OPTIONS'",
          "gatewayresponse.header.Access-Control-Allow-Origin" : "'${local.cors_allowed_origin}'",
          "gatewayresponse.header.Access-Control-Allow-Headers" : "'Content-Type,X-Amz-Date,Authorization,X-Api-Key,X-Amz-Security-Token'"
        }
      }
    }
  })

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_deployment" "frontend_api" {
  rest_api_id = aws_api_gateway_rest_api.frontend_api.id

  triggers = {
    redeployment = sha1(jsonencode(aws_api_gateway_rest_api.frontend_api.body))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "frontend_api" {
  deployment_id = aws_api_gateway_deployment.frontend_api.id
  rest_api_id   = aws_api_gateway_rest_api.frontend_api.id
  stage_name    = "frontend"
}

resource "aws_api_gateway_base_path_mapping" "frontend_api" {
  api_id      = aws_api_gateway_rest_api.frontend_api.id
  stage_name  = aws_api_gateway_stage.frontend_api.stage_name
  domain_name = aws_api_gateway_domain_name.frontend_api.domain_name
}

##############################################################
# Partner APIGW with mTLS
##############################################################
resource "aws_api_gateway_domain_name" "partner_api" {
  domain_name              = "api.${var.domain_name}"
  regional_certificate_arn = aws_acm_certificate.backend_beanstalk.arn

  endpoint_configuration {
    types = ["REGIONAL"]
  }

  mutual_tls_authentication {
    truststore_uri = "s3://miniconomy-trust-store-bucket/truststore.pem"
  }
}

resource "aws_api_gateway_rest_api" "partner_api" {
  name = "partner"
  body = jsonencode({
    "openapi" : "3.0.1",
    "info" : {
      "title" : "partner",
      "version" : "1.0"
    },
    "servers" : [{
      "url" : "https://api.insurance.projects.bbdgrad.com",
      "x-amazon-apigateway-endpoint-configuration" : {
        "disableExecuteApiEndpoint" : true
      }
    }],
    "paths" : {
      "/" : {
        "get" : {
          "x-amazon-apigateway-integration" : {
            "connectionId" : aws_api_gateway_vpc_link.nlb.id,
            "httpMethod" : "GET",
            "uri" : "http://${data.aws_lb.nlb.dns_name}",
            "passthroughBehavior" : "when_no_match",
            "connectionType" : "VPC_LINK",
            "type" : "http_proxy"
          }
        }
      },
      "/{proxy+}" : {
        "x-amazon-apigateway-any-method" : {
          "parameters" : [{
            "name" : "proxy",
            "in" : "path",
            "required" : true,
            "schema" : {
              "type" : "string"
            }
          }],
          "x-amazon-apigateway-integration" : {
            "connectionId" : aws_api_gateway_vpc_link.nlb.id,
            "httpMethod" : "ANY",
            "uri" : "http://${data.aws_lb.nlb.dns_name}/{proxy}",
            "passthroughBehavior" : "when_no_match",
            "connectionType" : "VPC_LINK",
            "type" : "http_proxy",
            "requestParameters" : {
              "integration.request.path.proxy" : "method.request.path.proxy"
            }
          }
        }
      }
    },
    "components" : {}
  })

  endpoint_configuration {
    types = ["REGIONAL"]
  }
}

resource "aws_api_gateway_deployment" "partner_api" {
  rest_api_id = aws_api_gateway_rest_api.partner_api.id

  triggers = {
    redeployment = sha1(jsonencode(aws_api_gateway_rest_api.partner_api.body))
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_api_gateway_stage" "partner_api" {
  deployment_id = aws_api_gateway_deployment.partner_api.id
  rest_api_id   = aws_api_gateway_rest_api.partner_api.id
  stage_name    = "partner"
}

resource "aws_api_gateway_base_path_mapping" "partner_api" {
  api_id      = aws_api_gateway_rest_api.partner_api.id
  stage_name  = aws_api_gateway_stage.partner_api.stage_name
  domain_name = aws_api_gateway_domain_name.partner_api.domain_name
}
