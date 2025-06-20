terraform {
  cloud {
    organization = "IgorKordiukiewicz"
    workspaces {
      name = "tasktracker"
    }
  }

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

resource "azurerm_resource_group" "rg" {
  name     = "${var.base_name}-rg"
  location = var.azure_location
}

module "database" {
  source = "./modules/database"

  supabase_organization = var.supabase_organization
  supabase_name = var.supabase_name
  supabase_location = var.supabase_location
}

module "storage" {
  source = "./modules/storage"

  base_name = var.base_name
  resource_group_name = azurerm_resource_group.rg.name
  azure_location = var.azure_location
  storage_container_name = var.storage_container_name
}

module "monitoring" {
  source = "./modules/monitoring"

  base_name = var.base_name
  resource_group_name = azurerm_resource_group.rg.name
  azure_location = var.azure_location
}

module "compute" {
  source = "./modules/compute"

  base_name = var.base_name
  resource_group_name = azurerm_resource_group.rg.name
  azure_location = var.azure_location
  storage_container_name = var.storage_container_name
  db_password = module.database.db_password
  supabase_project_id = module.database.supabase_project_id
  insights_connection_string = module.monitoring.insights_connection_string
  storage_connection_string = module.storage.storage_account_connection-string
}