variable "base_name" {
  type        = string
  description = "Base name for the resources names."
}

variable "resource_group_name" {
  type        = string
  description = "Resource group name."
}

variable "azure_location" {
  default     = "westeurope"
  description = "Location of the Azure resources."
}

variable "storage_container_name" {
  type        = string
  description = "Name of the main storage container."
}
