terraform {
  required_providers {
    supabase = {
      source = "supabase/supabase"
      version = "~> 1.0"
    }
  }
}

resource "random_password" "db_password" {
  length  = 16
  special = true
}

resource "supabase_project" "supabase" {
  organization_id   = var.supabase_organization
  name              = var.supabase_name
  database_password = random_password.db_password.result
  region            = var.supabase_location

  lifecycle {
    ignore_changes = [database_password]
  }
}
