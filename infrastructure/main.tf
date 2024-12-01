terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.10.0"
    }

    supabase = {
      source = "supabase/supabase"
      version = "~> 1.0"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
}

provider "supabase" {
  access_token = var.supabase_token
}

resource "random_password" "db_password" {
  length = 16
  special = true
}

resource "random_password" "hangfire_password" {
  length = 16
  special = true
}

resource "supabase_project" "supabase" {
  organization_id = var.supabase_organization
  name = "TaskTracker1"
  database_password = random_password.db_password.result
  region = "eu-central-1"

  lifecycle {
    ignore_changes = [database_password]
  }
}

resource "azurerm_resource_group" "rg" {
  name     = "${var.baseName}-rg"
  location = var.location
}

resource "azurerm_service_plan" "serviceplan" {
  name = "${var.baseName}-serviceplan"
  location = var.location
  resource_group_name = azurerm_resource_group.rg.name
  sku_name = "F1"
  os_type = "Windows"
}

resource "azurerm_windows_web_app" "api" {
  name = "${var.baseName}-api"
  location = var.location
  resource_group_name = azurerm_resource_group.rg.name
  service_plan_id = azurerm_service_plan.serviceplan.id
  site_config {
      always_on = false
  }

  app_settings = {
    "ConnectionStrings__DefaultConnection" = format("User Id=postgres.%s;Password=%s;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;", supabase_project.supabase.id, random_password.db_password.result)
    "ConnectionStrings__AppInsightsConnection" = azurerm_application_insights.appinsights.connection_string
    "Authentication__Issuer" = format("https://%s.supabase.co/auth/v1", supabase_project.supabase.id)
    "Authentication__HangfirePassword" = random_password.hangfire_password.result
  }
}

resource "azurerm_log_analytics_workspace" "law" {
  name = "${var.baseName}-law"
  location = var.location
  resource_group_name = azurerm_resource_group.rg.name
  sku = "PerGB2018"
}

resource "azurerm_application_insights" "appinsights" {
  name = "${var.baseName}-appinsights"
  location = var.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type = "web"
  workspace_id = azurerm_log_analytics_workspace.law.id
}