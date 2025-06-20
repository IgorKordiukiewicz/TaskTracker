variable "supabase_organization" {
  type        = string
  description = "Id of the Supabase organization (slug)."
}

variable "supabase_name" {
  type        = string
  description = "Name of the supabase project."
}

variable "supabase_location" {
  type        = string
  default     = "eu-central-1"
  description = "Location of the Supabase project."
}
