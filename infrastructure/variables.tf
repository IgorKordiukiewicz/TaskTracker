variable "azure_subscription_id" {
  type = string
}

variable "baseName" {
  default     = "tasktracker"
  description = "Base name for the resources names."
}

variable "location" {
  default     = "westeurope"
  description = "Location of the resources."
}

variable "supabase_token" {
  type = string
}

variable "supabase_organization" {
  type = string
}