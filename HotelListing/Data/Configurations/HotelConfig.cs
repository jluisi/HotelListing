using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Data.Configurations
{
  public class HotelConfig : IEntityTypeConfiguration<Hotel>
  {
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
      //---------------------------------------------------------------------------
      // 1. Table Name - Pluralized in SQL Server
      //---------------------------------------------------------------------------
      builder.ToTable("Hotels");

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
        new Hotel
        {
          Id = 1,
          Name = "Sandals Resort and Spa",
          Address = "Negril",
          Rating = 4.5,
          CountryId = 1
        },
        new Hotel
        {
          Id = 2,
          Name = "Grand Palladium",
          Address = "Nassau",
          Rating = 4,
          CountryId = 2
        },
        new Hotel
        {
          Id = 3,
          Name = "Comfort Suites",
          Address = "Georgetown",
          Rating = 4.3,
          CountryId = 3
        }
      );
    }

  }
}

