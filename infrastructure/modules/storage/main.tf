resource "azurerm_storage_account" "storage_account" {
  name                     = "${var.base_name}storageacc"
  resource_group_name      = var.resource_group_name
  location                 = var.azure_location
  account_replication_type = "LRS"
  account_tier             = "Standard"
}

resource "azurerm_storage_container" "storage_container" {
  name                  = var.storage_container_name
  storage_account_id    = azurerm_storage_account.storage_account.id
  container_access_type = "private"
}
