resource "azurerm_log_analytics_workspace" "law" {
  name                = "${var.base_name}-law"
  location            = var.azure_location
  resource_group_name = var.resource_group_name
  sku                 = "PerGB2018"
}

resource "azurerm_application_insights" "appinsights" {
  name                = "${var.base_name}-appinsights"
  location            = var.azure_location
  resource_group_name = var.resource_group_name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.law.id
}
