﻿namespace BlazorApp.Models {
	public record Country {
		public string country_name { get; set; }
		public string country_short_name { get; set; }
		public int country_phone_code { get; set; }
	}
}
