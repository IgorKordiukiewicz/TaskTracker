resource "random_password" "hangfire_password" {
  length  = 16
  special = true
}

resource "azurerm_service_plan" "serviceplan" {
  name                = "${var.base_name}-serviceplan"
  location            = var.azure_location
  resource_group_name = var.resource_group_name
  sku_name            = "F1"
  os_type             = "Windows"
}

resource "azurerm_windows_web_app" "api" {
  name                = "${var.base_name}-api"
  location            = var.azure_location
  resource_group_name = var.resource_group_name
  service_plan_id     = azurerm_service_plan.serviceplan.id

  site_config {
    always_on = false
  }

  app_settings = {
    "ConnectionStrings__DefaultConnection"     = format("User Id=postgres.%s;Password=%s;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;", var.supabase_project_id, var.db_password)
    "ConnectionStrings__AnalyticsConnection"   = format("User Id=postgres.%s;Password=%s;Server=aws-0-eu-central-1.pooler.supabase.com;Port=5432;Database=postgres;", var.supabase_project_id, var.db_password)
    "ConnectionStrings__AppInsightsConnection" = var.insights_connection_string
    "ConnectionStrings__BlobStorageConnection" = var.storage_connection_string
    "InfrastructureSettings__Blob__Container"  = var.storage_container_name
    "Authentication__Issuer"                   = format("https://%s.supabase.co/auth/v1", var.supabase_project_id)
    "Authentication__HangfirePassword"         = random_password.hangfire_password.result
  }

  lifecycle {
    ignore_changes = [
      app_settings["Authentication__JwtSecret"],
      app_settings["ConfigurationSettings__Domain"]
    ]
  }
}
