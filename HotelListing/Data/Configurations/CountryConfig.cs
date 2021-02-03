using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations
{
  public class CountryConfig : IEntityTypeConfiguration<Country>
  {
    public void Configure(EntityTypeBuilder<Country> builder)
    {
      //---------------------------------------------------------------------------
      // 1. Table Name - Pluralized in SQL Server
      //---------------------------------------------------------------------------
      builder.ToTable("Countries");

      //---------------------------------------------------------------------------
      // 2. Primary Key
      //---------------------------------------------------------------------------
      builder.HasKey(e => e.Id);

      //---------------------------------------------------------------------------
      // 3. Properties in Alphabetical Order
      //---------------------------------------------------------------------------
      builder.Property(e => e.Name)
        .HasMaxLength(100);

      //---------------------------------------------------------------------------
      // 4. Relationships in Alphabetical Order
      //---------------------------------------------------------------------------

      //---------------------------------------------------------------------------
      // 5. Seed Data
      //---------------------------------------------------------------------------
      builder.HasData(
        new Country { Id = 1, Name = "Jamaica", ShortName = "JM" },
        new Country { Id = 2, Name = "Bahamas", ShortName = "BS" },
        new Country { Id = 3, Name = "Cayman Island", ShortName = "CI" }
      );
    }

  }
}

