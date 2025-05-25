variable "azure_subscription_id" {
  type = string
  description = "Azure subscription id."
}

variable "baseName" {
  default     = "tasktracker"
  description = "Base name for the resources names."
}

variable "azure_location" {
  default     = "westeurope"
  description = "Location of the Azure resources."
}

variable "supabase_location" {
  default = "eu-central-1"
  description = "Location of the Supabase project."
}

variable "supabase_name" {
  default = "TaskTracker"
  description = "Name of the supabase project."
}

variable "supabase_token" {
  type = string
  description = "Supabase access token."
}

variable "supabase_organization" {
  type = string
  description = "Id of the Supabase organization (slug)."
}