﻿namespace HNSHOP.Dtos.Request
{
    public class VerifyEmailReqDto
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
