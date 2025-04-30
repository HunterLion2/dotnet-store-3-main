namespace dotnet_store.Models;
using System.ComponentModel.DataAnnotations;

public class UrunEditModel: UrunModel {

    
    public string? ResimAdi { get; set; }

    public int Id { get; set; }
}

