using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CashCompass.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgresMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Create Tables (Auto-Generated)
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    CategoryName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_Categories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IncomeSource",
                columns: table => new
                {
                    IncomeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    SourceName = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    PayFrequency = table.Column<string>(type: "text", nullable: false),
                    NextPayDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IncomeSource", x => x.IncomeId);
                    table.ForeignKey(
                        name: "FK_IncomeSource_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    ExpenseName = table.Column<string>(type: "text", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Notes = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expenses_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Expenses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Allocations",
                columns: table => new
                {
                    AllocationId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    IncomeId = table.Column<int>(type: "integer", nullable: false),
                    CategoryId = table.Column<int>(type: "integer", nullable: false),
                    AllocationType = table.Column<string>(type: "text", nullable: false),
                    AllocationValue = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Allocations", x => x.AllocationId);
                    table.ForeignKey(
                        name: "FK_Allocations_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Allocations_IncomeSource_IncomeId",
                        column: x => x.IncomeId,
                        principalTable: "IncomeSource",
                        principalColumn: "IncomeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Allocations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            // =========================================================================
            //  MANUAL SQL: STORED FUNCTIONS FOR GRAPHQL MUTATIONS
            // =========================================================================
            migrationBuilder.Sql(@"
                -- 1. Setup pgcrypto for password hashing
                CREATE EXTENSION IF NOT EXISTS pgcrypto;

                ----------------------------------------------------------
                -- USER FUNCTIONS
                ----------------------------------------------------------
                CREATE OR REPLACE FUNCTION create_user(
                    p_fullname TEXT,
                    p_email TEXT,
                    p_password TEXT 
                )
                RETURNS SETOF ""Users""
                LANGUAGE plpgsql
                AS $$
                DECLARE
                    v_password_hash TEXT;
                BEGIN
                    v_password_hash := crypt(p_password, gen_salt('bf', 8));

                    RETURN QUERY
                    INSERT INTO ""Users"" (""FullName"", ""Email"", ""PasswordHash"", ""CreatedAt"")
                    VALUES (p_fullname, p_email, v_password_hash, NOW())
                    RETURNING ""UserId"", ""FullName"", ""Email"", ""CreatedAt"";
                END;
                $$;

                CREATE OR REPLACE FUNCTION update_user(
                    p_userid INT,
                    p_fullname TEXT,
                    p_email TEXT
                )
                RETURNS SETOF ""Users""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""Users""
                    SET 
                        ""FullName"" = p_fullname,
                        ""Email"" = p_email
                    WHERE ""UserId"" = p_userid;

                    RETURN QUERY
                    SELECT ""UserId"", ""FullName"", ""Email"", ""CreatedAt""
                    FROM ""Users""
                    WHERE ""UserId"" = p_userid;
                END;
                $$;

                CREATE OR REPLACE FUNCTION delete_user(
                    p_userid INT
                )
                RETURNS VOID
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    DELETE FROM ""Users""
                    WHERE ""UserId"" = p_userid;
                END;
                $$;

                ----------------------------------------------------------
                -- INCOME SOURCE FUNCTIONS
                ----------------------------------------------------------
                CREATE OR REPLACE FUNCTION create_income_source(
                    p_userid INT,
                    p_sourcename TEXT,
                    p_amount NUMERIC,
                    p_payfrequency TEXT,
                    p_nextpaydate TIMESTAMP WITHOUT TIME ZONE
                )
                RETURNS SETOF ""IncomeSource""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RETURN QUERY
                    INSERT INTO ""IncomeSource"" (""UserId"", ""SourceName"", ""Amount"", ""PayFrequency"", ""NextPayDate"", ""CreatedAt"")
                    VALUES (p_userid, p_sourcename, p_amount, p_payfrequency, p_nextpaydate, NOW())
                    RETURNING *;
                END;
                $$;

                CREATE OR REPLACE FUNCTION update_income_source(
                    p_incomeid INT,
                    p_sourcename TEXT,
                    p_amount NUMERIC,
                    p_payfrequency TEXT,
                    p_nextpaydate TIMESTAMP WITHOUT TIME ZONE
                )
                RETURNS SETOF ""IncomeSource""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""IncomeSource""
                    SET 
                        ""SourceName"" = p_sourcename,
                        ""Amount"" = p_amount,
                        ""PayFrequency"" = p_payfrequency,
                        ""NextPayDate"" = p_nextpaydate
                    WHERE ""IncomeId"" = p_incomeid;

                    RETURN QUERY
                    SELECT *
                    FROM ""IncomeSource""
                    WHERE ""IncomeId"" = p_incomeid;
                END;
                $$;

                CREATE OR REPLACE FUNCTION delete_income_source(
                    p_incomeid INT
                )
                RETURNS VOID
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    DELETE FROM ""IncomeSource""
                    WHERE ""IncomeId"" = p_incomeid;
                END;
                $$;

                ----------------------------------------------------------
                -- CATEGORY FUNCTIONS
                ----------------------------------------------------------
                CREATE OR REPLACE FUNCTION create_category(
                    p_userid INT,
                    p_categoryname TEXT,
                    p_description TEXT
                )
                RETURNS SETOF ""Categories""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RETURN QUERY
                    INSERT INTO ""Categories"" (""UserId"", ""CategoryName"", ""Description"")
                    VALUES (p_userid, p_categoryname, p_description)
                    RETURNING *;
                END;
                $$;

                CREATE OR REPLACE FUNCTION update_category(
                    p_categoryid INT,
                    p_categoryname TEXT,
                    p_description TEXT
                )
                RETURNS SETOF ""Categories""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""Categories""
                    SET 
                        ""CategoryName"" = p_categoryname,
                        ""Description"" = p_description
                    WHERE ""CategoryId"" = p_categoryid;

                    RETURN QUERY
                    SELECT *
                    FROM ""Categories""
                    WHERE ""CategoryId"" = p_categoryid;
                END;
                $$;

                CREATE OR REPLACE FUNCTION delete_category(
                    p_categoryid INT
                )
                RETURNS VOID
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    DELETE FROM ""Categories""
                    WHERE ""CategoryId"" = p_categoryid;
                END;
                $$;

                ----------------------------------------------------------
                -- EXPENSE FUNCTIONS
                ----------------------------------------------------------
                CREATE OR REPLACE FUNCTION create_expense(
                    p_userid INT,
                    p_expensename TEXT,
                    p_amount NUMERIC,
                    p_categoryid INT,
                    p_expensedate TIMESTAMP WITHOUT TIME ZONE,
                    p_notes TEXT
                )
                RETURNS SETOF ""Expenses""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RETURN QUERY
                    INSERT INTO ""Expenses"" (""UserId"", ""ExpenseName"", ""Amount"", ""CategoryId"", ""ExpenseDate"", ""Notes"", ""CreatedAt"")
                    VALUES (p_userid, p_expensename, p_amount, p_categoryid, p_expensedate, p_notes, NOW())
                    RETURNING *;
                END;
                $$;

                CREATE OR REPLACE FUNCTION update_expense(
                    p_expenseid INT,
                    p_expensename TEXT,
                    p_amount NUMERIC,
                    p_categoryid INT,
                    p_expensedate TIMESTAMP WITHOUT TIME ZONE,
                    p_notes TEXT
                )
                RETURNS SETOF ""Expenses""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""Expenses""
                    SET 
                        ""ExpenseName"" = p_expensename,
                        ""Amount"" = p_amount,
                        ""CategoryId"" = p_categoryid,
                        ""ExpenseDate"" = p_expensedate,
                        ""Notes"" = p_notes
                    WHERE ""ExpenseId"" = p_expenseid;

                    RETURN QUERY
                    SELECT *
                    FROM ""Expenses""
                    WHERE ""ExpenseId"" = p_expenseid;
                END;
                $$;

                CREATE OR REPLACE FUNCTION delete_expense(
                    p_expenseid INT
                )
                RETURNS VOID
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    DELETE FROM ""Expenses""
                    WHERE ""ExpenseId"" = p_expenseid;
                END;
                $$;

                ----------------------------------------------------------
                -- ALLOCATION FUNCTIONS
                ----------------------------------------------------------
                CREATE OR REPLACE FUNCTION create_allocation(
                    p_userid INT,
                    p_incomeid INT,
                    p_categoryid INT,
                    p_allocationtype TEXT,
                    p_allocationvalue NUMERIC
                )
                RETURNS SETOF ""Allocations""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    RETURN QUERY
                    INSERT INTO ""Allocations"" (""UserId"", ""IncomeId"", ""CategoryId"", ""AllocationType"", ""AllocationValue"", ""CreatedAt"")
                    VALUES (p_userid, p_incomeid, p_categoryid, p_allocationtype, p_allocationvalue, NOW())
                    RETURNING *;
                END;
                $$;

                CREATE OR REPLACE FUNCTION update_allocation(
                    p_allocationid INT,
                    p_allocationtype TEXT,
                    p_allocationvalue NUMERIC
                )
                RETURNS SETOF ""Allocations""
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    UPDATE ""Allocations""
                    SET 
                        ""AllocationType"" = p_allocationtype,
                        ""AllocationValue"" = p_allocationvalue
                    WHERE ""AllocationId"" = p_allocationid;

                    RETURN QUERY
                    SELECT *
                    FROM ""Allocations""
                    WHERE ""AllocationId"" = p_allocationid;
                END;
                $$;

                CREATE OR REPLACE FUNCTION delete_allocation(
                    p_allocationid INT
                )
                RETURNS VOID
                LANGUAGE plpgsql
                AS $$
                BEGIN
                    DELETE FROM ""Allocations""
                    WHERE ""AllocationId"" = p_allocationid;
                END;
                $$;
            ");
            // =========================================================================
            //  END MANUAL SQL
            // =========================================================================

            // 3. Create Indexes (Auto-Generated)
            migrationBuilder.CreateIndex(
                name: "IX_Allocations_CategoryId",
                table: "Allocations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_IncomeId",
                table: "Allocations",
                column: "IncomeId");

            migrationBuilder.CreateIndex(
                name: "IX_Allocations_UserId",
                table: "Allocations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_UserId",
                table: "Categories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CategoryId",
                table: "Expenses",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_UserId",
                table: "Expenses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IncomeSource_UserId",
                table: "IncomeSource",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Allocations");

            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "IncomeSource");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}