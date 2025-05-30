﻿using Avalonia.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskMonagerAviaProject.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public byte[]? UserImage { get; set; }
        public byte[]? ImageBackground { get; set; } = null;
        public bool? DarckLightColor { get; set; } = null!;
    }
}
