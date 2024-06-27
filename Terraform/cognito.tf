resource "aws_secretsmanager_secret" "auth_secret_key" {
  name = "auth_secret_key"
}

resource "aws_secretsmanager_secret_version" "auth_secret_key" {
  secret_id     = aws_secretsmanager_secret.auth_secret_key.id
  secret_string = "dummy"
}

data "aws_secretsmanager_secret_version" "auth_secret_key" {
  secret_id  = aws_secretsmanager_secret.auth_secret_key.arn
  depends_on = [aws_secretsmanager_secret_version.auth_secret_key]
}

resource "aws_cognito_user_pool" "prd_pool" {
  name                     = var.db_name
  alias_attributes         = ["email"]
  auto_verified_attributes = ["email"]
}

resource "aws_cognito_user_pool_domain" "prd_pool" {
  domain       = "business-expense-manager"
  user_pool_id = aws_cognito_user_pool.prd_pool.id
}

resource "aws_cognito_identity_provider" "google" {
  user_pool_id  = aws_cognito_user_pool.prd_pool.id
  provider_name = "Google"
  provider_type = "Google"

  provider_details = {
    authorize_scopes              = "email openid"
    client_id                     = var.google_client_id
    client_secret                 = data.aws_secretsmanager_secret_version.auth_secret_key.secret_string
    token_url                     = "https://www.googleapis.com/oauth2/v4/token"
    token_request_method          = "POST"
    oidc_issuer                   = "https://accounts.google.com"
    authorize_url                 = "https://accounts.google.com/o/oauth2/v2/auth"
    attributes_url                = "https://people.googleapis.com/v1/people/me?personFields="
    attributes_url_add_attributes = "true"
  }

  attribute_mapping = {
    email          = "email"
    username       = "sub"
    email_verified = "email_verified"
  }
}

resource "aws_cognito_user_pool_client" "prd_pool" {
  name                         = "prd_client"
  user_pool_id                 = aws_cognito_user_pool.prd_pool.id
  access_token_validity        = 1
  supported_identity_providers = ["Google"]
  allowed_oauth_flows          = ["implicit"]
  allowed_oauth_scopes         = ["email", "openid"]
  callback_urls                = ["https://web.karle.co.za", "http://localhost:4200"]
  generate_secret              = true
}