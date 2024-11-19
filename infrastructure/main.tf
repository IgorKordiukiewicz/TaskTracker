terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm",
      version = "~> 4.10.0"
    }
  }

  required_version = ">= 1.1.0"
}

provider "azurerm" {
  features {}
  subscription_id = var.azure_subscription_id
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
}