output "storage_account_connection-string" {
  value       = azurerm_storage_account.storage_account.primary_connection_string
  sensitive   = true
  description = "Storage Account primary connection string."
}

output "storage_container_name" {
  value       = var.storage_container_name
  description = "Name of the main storage container."
}
