output "db_password" {
  value       = random_password.db_password.result
  sensitive   = true
  description = "Generated database password."
}

output "supabase_project_id" {
  value       = supabase_project.supabase.id
  description = "Supabase project ID."
}
