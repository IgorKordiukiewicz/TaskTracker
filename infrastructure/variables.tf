variable "azure_subscription_id" {
  type        = string
  description = "Azure subscription id."
}

variable "base_name" {
  type        = string
  description = "Base name for the resources names."
}

variable "azure_location" {
  type        = string
  default     = "westeurope"
  description = "Location of the Azure resources."
}

variable "supabase_location" {
  type        = string
  default     = "eu-central-1"
  description = "Location of the Supabase project."
}

variable "supabase_name" {
  type        = string
  description = "Name of the supabase project."
}

variable "supabase_token" {
  type        = string
  sensitive   = true
  description = "Supabase access token."
}

variable "supabase_organization" {
  type        = string
  description = "Id of the Supabase organization (slug)."
}

variable "storage_container_name" {
  type        = string
  description = "Name of the main storage container."
}
