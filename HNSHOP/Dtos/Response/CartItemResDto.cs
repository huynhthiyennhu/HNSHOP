﻿namespace HNSHOP.Dtos.Response
{
    public class CartItemResDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; } = string.Empty;
    }
}
