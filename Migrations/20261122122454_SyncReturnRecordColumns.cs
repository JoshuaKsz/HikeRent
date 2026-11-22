using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HikeRent.Migrations
{
    /// <inheritdoc />
    public partial class SyncReturnRecordColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // DB lokal lama punya "RentalFee" dan "Condition".
            // DB fresh dari migration punya "DamageFee" dan "Condition".
            // Samakan keduanya ke "DamageFee" dan buang "Condition".
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'RentalFee'
    ) AND NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'DamageFee'
    ) THEN
        ALTER TABLE ""ReturnRecords"" RENAME COLUMN ""RentalFee"" TO ""DamageFee"";
    ELSIF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'RentalFee'
    ) AND EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'DamageFee'
    ) THEN
        UPDATE ""ReturnRecords""
        SET ""DamageFee"" = ""RentalFee""
        WHERE ""DamageFee"" = 0 AND ""RentalFee"" <> 0;
        ALTER TABLE ""ReturnRecords"" DROP COLUMN ""RentalFee"";
    END IF;

    IF EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'Condition'
    ) THEN
        ALTER TABLE ""ReturnRecords"" DROP COLUMN ""Condition"";
    END IF;
END $$;
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'public' AND table_name = 'ReturnRecords' AND column_name = 'Condition'
    ) THEN
        ALTER TABLE ""ReturnRecords""
        ADD COLUMN ""Condition"" character varying(50) NOT NULL DEFAULT '';
        ALTER TABLE ""ReturnRecords"" ALTER COLUMN ""Condition"" DROP DEFAULT;
    END IF;
END $$;
");
        }
    }
}
