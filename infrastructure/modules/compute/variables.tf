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

variable "db_password" {
  type        = string
  description = "Generated database password."
  sensitive   = true
}

variable "supabase_project_id" {
  type        = string
  description = "Id of the supabase project."
}

variable "insights_connection_string" {
  type        = string
  description = "Application Insights connection string."
  sensitive   = true
}

variable "storage_connection_string" {
  type        = string
  description = "Storage Account connection string."
  sensitive   = true
}
