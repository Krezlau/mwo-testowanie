﻿namespace mwo_testowanie.Models.DTOs;

public class ProductDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int QuantityLeft { get; set; }
}