﻿using BlazorApp.Enums;

namespace BlazorApp.Models {
	public class User {
		public int? Id { get; set; }
		public string Name { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public UserType UserType { get; set; }
		public IFormFile Photo { get; set; }
	}
}
