data "aws_secretsmanager_secret_version" "db_details" {
  secret_id = module.rds.db_instance_master_user_secret_arn
}

resource "aws_s3_object" "backend_beanstalk_default" {
  bucket = aws_s3_bucket.backend_beanstalk.id
  key    = "default_api.zip"
  source = "./DummyVersions/default_api.zip"
}

resource "aws_elastic_beanstalk_application_version" "default" {
  name         = "tf-test"
  application  = "api-app"
  description  = "application version created by terraform"
  bucket       = aws_s3_bucket.backend_beanstalk.id
  key          = aws_s3_object.backend_beanstalk_default.id
  force_delete = true
}