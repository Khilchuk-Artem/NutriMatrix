using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FoodCatalog.Api.Migrations
{
    /// <inheritdoc />
    public partial class Addnutrientseeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nutrients",
                columns: new[] { "Id", "IsDeleted", "Name", "Unit" },
                values: new object[,]
                {
                    { new Guid("00187677-4fd7-4640-bfa2-64f492ca0d88"), false, "6:00", "g" },
                    { new Guid("01d61688-924e-4a3d-8e39-1caca84e0844"), false, "Cystine", "g" },
                    { new Guid("02868c37-7a2d-40af-bcd5-aa9ba05933c5"), false, "20:4 n-6", "g" },
                    { new Guid("031d7115-a0b5-4663-8fae-cd2d54d5860f"), false, "18:2 CLAs", "g" },
                    { new Guid("08e0d1ce-5fb3-4780-a9e7-878a6da46ef4"), false, "Methionine", "g" },
                    { new Guid("09a87134-6acd-433f-9f22-82949ad5878e"), false, "18:2 i", "g" },
                    { new Guid("09b254c2-6196-4d4a-bfb2-3acd9f5fd9f8"), false, "Vitamin D3 (cholecalciferol)", "Âµg" },
                    { new Guid("09ec2628-6c48-42ac-b3c7-a6ce53a4686c"), false, "Carbohydrate, by difference", "g" },
                    { new Guid("0a8f83a8-225b-4332-97da-1b39ff2eaf5d"), false, "Water", "g" },
                    { new Guid("0e1d39c8-500e-468a-8799-7497753a9186"), false, "Tyrosine", "g" },
                    { new Guid("0e47721a-14b9-4174-a73d-5515cfbd00c6"), false, "Retinol", "Âµg" },
                    { new Guid("0f3cfcbf-1eb2-411c-aec9-a779bced180c"), false, "Fatty acids, total saturated", "g" },
                    { new Guid("0fd0c57b-2b41-40a5-9c57-11085f19838c"), false, "Energy", "kcal" },
                    { new Guid("1143c769-af9d-4183-8907-c46df3d408a6"), false, "Lycopene", "Âµg" },
                    { new Guid("13556c89-cc9c-4503-8254-8b519cfed2bb"), false, "Choline, total", "mg" },
                    { new Guid("145d0bbc-b6b9-4d6b-acd2-de7a5b382f58"), false, "Fluoride, F", "Âµg" },
                    { new Guid("15c90b30-4bf1-49b5-9149-ef09874044e9"), false, "18:1 c", "g" },
                    { new Guid("180dd2ef-e910-4e7f-a8ab-cb41474d63b0"), false, "Sugars, total", "g" },
                    { new Guid("195c204c-9a3a-48a9-a4ad-ae9fac6352fa"), false, "Isomalt", "g" },
                    { new Guid("1a000942-9789-4ed0-8f68-84c168dc716b"), false, "Selenium, Se", "Âµg" },
                    { new Guid("1a5ffa66-c3bc-4989-b863-e3469dcae1de"), false, "Beta-sitosterol", "mg" },
                    { new Guid("1d6e9df3-d6fd-47c0-82ce-57651eb1446a"), false, "4:00", "g" },
                    { new Guid("213bfe86-6c2b-493f-a03f-a1d4fc58b2a0"), false, "15:00", "g" },
                    { new Guid("23b00007-ee3a-451d-9c8a-4bc238654f1d"), false, "22:1 c", "g" },
                    { new Guid("27a06531-a18d-43fa-8cb0-5120f346dfc1"), false, "Fatty acids, total polyunsaturated", "g" },
                    { new Guid("285681ee-6908-4887-9034-34dc2aab51d4"), false, "Fatty acids, total trans", "g" },
                    { new Guid("298693dd-534c-4dcf-8755-f0df1483142e"), false, "Allulose", "g" },
                    { new Guid("299364c5-1ca0-48aa-964f-149c39c75763"), false, "Stigmasterol", "mg" },
                    { new Guid("2a91fd10-edd4-406c-b07b-8997dc5ee550"), false, "Starch", "g" },
                    { new Guid("2c139fd7-981a-4962-a4e5-e380aaa7e1d1"), false, "20:2 n-6 c,c", "g" },
                    { new Guid("2c30679f-bf99-469c-9035-f9a973148b57"), false, "Sodium, Na", "mg" },
                    { new Guid("2f10c771-cd87-4579-8211-2099dacc598e"), false, "Fatty acids, total monounsaturated", "g" },
                    { new Guid("3048e404-f677-469c-9aef-ec5ad83158d2"), false, "Riboflavin", "mg" },
                    { new Guid("3126187d-5a90-4f94-b6a4-2bfa5a65eab7"), false, "18:2 n-6 c,c", "g" },
                    { new Guid("31a5fc29-515e-4ac6-bc5c-018a5d5460dd"), false, "Betaine", "mg" },
                    { new Guid("32cdc623-d7be-4f06-ab89-f071585d8c1c"), false, "Cryptoxanthin, beta", "Âµg" },
                    { new Guid("34a990a6-ecb2-4693-b6a2-7847dc5fe645"), false, "17:01", "g" },
                    { new Guid("35159dfe-2b2e-4859-adb8-7d17c2df8ddf"), false, "Alanine", "g" },
                    { new Guid("35435ac0-5216-469c-abe1-bf47088650ea"), false, "Sugar Alcohol", "g" },
                    { new Guid("3bcaac8e-2033-4dfe-bc77-f3dac53d3adb"), false, "20:3 n-3", "g" },
                    { new Guid("3d6f2328-3da1-4f43-aa43-0bd4ff9356a8"), false, "Threonine", "g" },
                    { new Guid("3f8bd51b-531f-480c-a8d8-9172d721088b"), false, "Tocopherol, gamma", "mg" },
                    { new Guid("3fca3bf0-53ee-4b0a-be1d-25a70d3f5b48"), false, "Lactose", "g" },
                    { new Guid("40eb533c-d9b9-420e-b57f-0d3554c3b7ef"), false, "Caffeine", "mg" },
                    { new Guid("41880e48-1311-41c7-91d5-5eae5bb78e6b"), false, "Manganese, Mn", "mg" },
                    { new Guid("41e2b244-230a-4b54-b4ec-25db26ee7b4c"), false, "Alcohol, ethyl", "g" },
                    { new Guid("42e2a0b2-cbbc-4f7c-a6a4-3bd73c53412f"), false, "Iron, Fe", "mg" },
                    { new Guid("43e78938-72f8-4234-bc1d-8722678ca10f"), false, "16:1 t", "g" },
                    { new Guid("483fbbce-543e-494f-a71f-757c4683c7c3"), false, "20:01", "g" },
                    { new Guid("4a12bfd9-0157-45df-9a10-1f2a6e3811a1"), false, "22:5 n-3 (DPA)", "g" },
                    { new Guid("4c9ecaa3-4382-4f38-acce-a39833120c7e"), false, "Histidine", "g" },
                    { new Guid("4f2fd50b-0840-451d-a991-f174175bd456"), false, "Copper, Cu", "mg" },
                    { new Guid("5a49107d-53b9-412d-8a80-40fe0397fc02"), false, "18:3 n-3 c,c,c (ALA)", "g" },
                    { new Guid("5b1d3996-f0aa-40ce-afab-0535983f3f71"), false, "Fructose", "g" },
                    { new Guid("5b52676a-741b-49d8-a842-e8b63fcae889"), false, "Tocotrienol, beta", "mg" },
                    { new Guid("5bcc8cdf-9ea9-477e-96dc-54e4fa8bfed3"), false, "22:00", "g" },
                    { new Guid("5ca3812b-d177-4a3e-8948-e45e1b5f2619"), false, "Carotene, alpha", "Âµg" },
                    { new Guid("5e1e5a94-47da-44e4-b1cb-2fec4d6cf81d"), false, "Phytosterols", "mg" },
                    { new Guid("5fa842ab-7781-4bf1-8065-8debfaa5b811"), false, "Tocopherol, beta", "mg" },
                    { new Guid("603875d4-a309-4e39-922c-37218fa1974c"), false, "Niacin", "mg" },
                    { new Guid("62fdf1a3-209b-4e74-9ed6-0a2da6df10c0"), false, "Vitamin D", "IU" },
                    { new Guid("657a8a9f-bcf2-48bd-ac29-f1efa7fb0701"), false, "Glycine", "g" },
                    { new Guid("66db2a18-bcff-4797-ae0d-65a8a3bc41a1"), false, "Total fat", "g" },
                    { new Guid("6adcbb0e-f797-4116-877d-2ba61c7ec2c8"), false, "Glucose (dextrose)", "g" },
                    { new Guid("6b597ae6-0dd2-4c15-91c7-d96be4767274"), false, "Vitamin A, RAE", "Âµg" },
                    { new Guid("6fb27a04-781e-49d5-a0cb-c25d7c78e7df"), false, "Sugars, added", "g" },
                    { new Guid("711914de-5c17-4eb0-8749-b15912c6a634"), false, "20:5 n-3 (EPA)", "g" },
                    { new Guid("71771f9e-ab46-42eb-9538-261a0ae70596"), false, "Hydroxyproline", "g" },
                    { new Guid("7386602a-8239-4c3e-a354-5cc1c665fe66"), false, "18:00", "g" },
                    { new Guid("78979ca6-2f2e-449d-901f-03dc233b48d4"), false, "Carotene, beta", "Âµg" },
                    { new Guid("7b93f187-5386-40b3-a053-aea6548e8eb5"), false, "Isoleucine", "g" },
                    { new Guid("7e539245-c33e-4bc9-ab3e-09a3bce96a38"), false, "20:3 n-6", "g" },
                    { new Guid("7e581336-4f25-4866-8a8a-e22305a2e9e0"), false, "Leucine", "g" },
                    { new Guid("7e89fa22-068b-4439-8cf6-284fc943f4fe"), false, "Theobromine", "mg" },
                    { new Guid("82277d5b-19e9-4f3c-b2a5-2a7a001682bb"), false, "Potassium, K", "mg" },
                    { new Guid("833d08a9-8775-4187-84d3-278efe75a5f1"), false, "Tocotrienol,delta", "mg" },
                    { new Guid("880790e8-858f-49a3-ac0d-6816a03f0a8d"), false, "21:05", "g" },
                    { new Guid("88a4ff58-bd22-4e32-b169-c6d6ef67d2ee"), false, "Vitamin C, total ascorbic acid", "mg" },
                    { new Guid("8a8de098-e4af-4280-8121-172da7fa1653"), false, "Maltitol", "g" },
                    { new Guid("8b6a0565-530a-4ae4-8163-201c485620b7"), false, "Arginine", "g" },
                    { new Guid("8c3aca94-7a61-4cad-b97b-87f7ce9b7c26"), false, "Folate, total", "Âµg" },
                    { new Guid("8df11934-8272-474d-aedb-92ca6862384b"), false, "Adjusted Protein", "g" },
                    { new Guid("8ef2c8a9-c00e-40c2-89f1-111a41c6c174"), false, "Sucrose", "g" },
                    { new Guid("91347054-d516-4cfb-a876-e0293703f636"), false, "Lutein + zeaxanthin", "Âµg" },
                    { new Guid("92904bea-e00f-48ae-9403-488b0f7b3bdb"), false, "Calcium, Ca", "mg" },
                    { new Guid("9457e52c-76cc-4391-b0e7-28d028e7dc0d"), false, "14:00", "g" },
                    { new Guid("95ce08d0-d496-4f58-a704-9cc8ae00bae6"), false, "Folate, DFE", "Âµg" },
                    { new Guid("96e690a3-c48b-4a78-8f1b-5b456d197ddd"), false, "18:3 n-6 c,c,c", "g" },
                    { new Guid("9792947b-cd46-4d9e-b175-4ba09c460d0e"), false, "Ash", "g" },
                    { new Guid("986480b6-e2aa-489e-897c-81926320d4e5"), false, "Mannitol", "g" },
                    { new Guid("9d4b0a55-5022-4559-ab70-e26b8bd8f533"), false, "20:4 undifferentiated", "g" },
                    { new Guid("a1539430-c0d7-4c43-8c5d-4c2135b48e6e"), false, "8:00", "g" },
                    { new Guid("a3d19f95-1ae7-4ac5-9cd1-c39adb1d9683"), false, "18:1-11t (18:1t n-7)", "g" },
                    { new Guid("a7242e2f-9f8b-463c-9060-1aae8b42e887"), false, "18:3 undifferentiated", "g" },
                    { new Guid("a8e864c0-6961-450c-a521-17ba7a39223e"), false, "22:1 undifferentiated", "g" },
                    { new Guid("aa2f287c-cc85-4e96-8c6a-e1ca4e5ba4e6"), false, "Vitamin D (D2 + D3)", "Âµg" },
                    { new Guid("af9575b8-8158-46b0-8980-2bb3b03df6e9"), false, "Cholesterol", "mg" },
                    { new Guid("b0b77c9a-79c7-40f1-b7e4-72f34068288e"), false, "Phosphorus, P", "mg" },
                    { new Guid("b1ca0be3-46ea-4fc4-9ea4-f6b5460eea17"), false, "Protein", "g" },
                    { new Guid("b263a17b-f130-4689-93fc-417b2306b920"), false, "Erythritol", "g" },
                    { new Guid("b3a9e840-d577-4527-bd87-288f8c5c9958"), false, "18:04", "g" },
                    { new Guid("b5454db4-5500-418e-8baf-4e698e54e715"), false, "18:1 t", "g" },
                    { new Guid("b7ddacdd-33d9-4ffd-bf63-df5bf5a19c23"), false, "Energy", "kJ" },
                    { new Guid("b9af5c83-26ff-49ca-9057-6cb05da209b4"), false, "Thiamin", "mg" },
                    { new Guid("bc1ba0b5-0ca3-4096-80d9-563eb1e57d97"), false, "12:00", "g" },
                    { new Guid("bcdb9b56-8b3e-42ad-815e-2b7ee52ebc27"), false, "Folic acid", "Âµg" },
                    { new Guid("be1a8d9d-fd1e-4ebc-be95-abe5b1121d3d"), false, "18:3i", "g" },
                    { new Guid("bfb22e75-b296-4b79-8fe7-aa1fa049732e"), false, "18:2 t not further defined", "g" },
                    { new Guid("c050ffb9-f639-4dd6-b692-108a5ac19878"), false, "Tocotrienol, gamma", "mg" },
                    { new Guid("c0b38407-970f-4690-be8c-6b9735eef4fd"), false, "Lysine", "g" },
                    { new Guid("c4b978dd-9415-430b-8f96-37152b817447"), false, "18:2 t,t", "g" },
                    { new Guid("c53d74db-b6d0-4d04-a61e-cb87c3bdebb4"), false, "Vitamin E (alpha-tocopherol)", "mg" },
                    { new Guid("c59d802f-0cef-410c-87f1-e64b9e68c072"), false, "Fatty acids, total trans-polyenoic", "g" },
                    { new Guid("c5ea852b-65b3-43be-8b31-ab0bbabe6814"), false, "Fiber, total dietary", "g" },
                    { new Guid("c6507833-497d-4060-8db1-cbf638f5a803"), false, "Serine", "g" },
                    { new Guid("c79dfa14-77c6-483f-a7f0-26e442b9df28"), false, "Vitamin K (phylloquinone)", "Âµg" },
                    { new Guid("c8378c55-f327-4ff5-aa18-ca439a26b601"), false, "22:04", "g" },
                    { new Guid("c91397cf-b878-41e8-aad5-8ebc58af3857"), false, "24:00:00", "g" },
                    { new Guid("c952ffbd-bb70-4bb8-9bb3-5f64cc2a2a5a"), false, "22:1 t", "g" },
                    { new Guid("c9b361bc-228e-475e-8188-3a4314802351"), false, "Folate, food", "Âµg" },
                    { new Guid("cb760f8b-d0cf-4fce-9f9d-0c60167644ca"), false, "16:00", "g" },
                    { new Guid("cbf7cd31-2708-4956-b1c7-d12dcd691208"), false, "Zinc, Zn", "mg" },
                    { new Guid("cbfa7cdf-e781-4a8a-8b93-1fca326d7ea6"), false, "Vitamin B-12, added", "Âµg" },
                    { new Guid("cc5582a4-d536-43bb-8ddf-8a4a44cb8c9a"), false, "Xylitol", "g" },
                    { new Guid("cc9f1df6-c625-4e1b-890b-3804d4a18d21"), false, "22:6 n-3 (DHA)", "g" },
                    { new Guid("cd365c75-7129-4748-8af0-a8cbd505a0ec"), false, "Vitamin B-12", "Âµg" },
                    { new Guid("d17dd16e-4313-4fbe-b754-236d6844122f"), false, "Valine", "g" },
                    { new Guid("d226a906-793e-4687-bc45-aed98ddadf7d"), false, "20:3 undifferentiated", "g" },
                    { new Guid("d3198cc8-0dd0-4e72-acf0-256799619627"), false, "Galactose", "g" },
                    { new Guid("d4d13698-7c99-4560-972f-8df8d1bf50b8"), false, "Dihydrophylloquinone", "Âµg" },
                    { new Guid("d5481a82-d13f-4c60-b454-243111f03a49"), false, "Phenylalanine", "g" },
                    { new Guid("d68f7f64-0c63-4e0e-aa3a-93f97ec37b14"), false, "Glycerin", "g" },
                    { new Guid("db803808-12a1-4e5f-b9dc-a932f5cb6ad6"), false, "Aspartic acid", "g" },
                    { new Guid("dc62a44f-632a-4f56-adb9-51f8d0e6c896"), false, "Pantothenic acid", "mg" },
                    { new Guid("de113fea-6505-4bbe-9c8e-5f44d974e26d"), false, "24:1 c", "g" },
                    { new Guid("df660f92-ea89-4a90-b7bf-2bf090b84dad"), false, "13:00", "g" },
                    { new Guid("e0ac6740-8942-44b5-931d-fb396631ee40"), false, "Campesterol", "mg" },
                    { new Guid("e1232069-daf0-444b-9546-8728fd184bca"), false, "18:2 undifferentiated", "g" },
                    { new Guid("e1511fd1-3de1-45ed-832a-ee5b4d61f983"), false, "Proline", "g" },
                    { new Guid("e18bfbeb-fc4e-4d87-84a2-e28f855db851"), false, "20:00", "g" },
                    { new Guid("e35581d9-2c00-4182-b305-487e76de5204"), false, "Lactitol", "g" },
                    { new Guid("e5366684-4dd5-4342-91fa-b9e7602ac195"), false, "16:1 c", "g" },
                    { new Guid("e5b847c7-2bff-46e6-8b2d-147b76bcf043"), false, "Menaquinone-4", "Âµg" },
                    { new Guid("e6fd620d-1255-4cd2-b8e0-10b3f70c38b1"), false, "15:01", "g" },
                    { new Guid("ed279501-bbaf-40de-9d24-3de7e2738803"), false, "18:1 undifferentiated", "g" },
                    { new Guid("ed3485f6-f55a-4cc7-b652-b99d2c82f64b"), false, "Maltose", "g" },
                    { new Guid("eebf87fe-53a1-482c-9ec2-01e084dd5d74"), false, "Tryptophan", "g" },
                    { new Guid("eec306a8-2f88-4d65-8c9b-51cd9dcd2d06"), false, "Vitamin D2 (ergocalciferol)", "Âµg" },
                    { new Guid("eef6bebc-dbc5-4c85-b668-8c05bb26a3e8"), false, "Vitamin B-6", "mg" },
                    { new Guid("f234bbd7-1fe1-4ca9-a0ba-4f58c6a26e7c"), false, "Vitamin A, IU", "IU" },
                    { new Guid("f2bc3b0f-5fed-4037-aede-119755394488"), false, "10:00", "g" },
                    { new Guid("f350a432-e749-4a78-b4ef-a85ed7c6533e"), false, "14:01", "g" },
                    { new Guid("f71f0f17-00da-4a04-8942-c87a005f30c9"), false, "Vitamin E, added", "mg" },
                    { new Guid("f7362078-3adf-4cfd-accf-7ec71d59c353"), false, "Tocotrienol, alpha", "mg" },
                    { new Guid("f79ed152-05d5-4ac9-9b1b-81f4a28d8698"), false, "Glutamic acid", "g" },
                    { new Guid("f8ad92bd-5123-4741-b0c0-e24f1bcb0888"), false, "Fatty acids, total trans-monoenoic", "g" },
                    { new Guid("f9fa052e-ff54-40a0-90cf-f34257baf5f2"), false, "16:1 undifferentiated", "g" },
                    { new Guid("fb89788e-4fa2-4e4a-b211-74470af49b65"), false, "Tocopherol, delta", "mg" },
                    { new Guid("fd0715b9-c807-4106-bd41-82ad6e59ae24"), false, "17:00", "g" },
                    { new Guid("fdec4a36-3c31-43a8-b96b-22aa746b892d"), false, "Sorbitol", "g" },
                    { new Guid("ff56dd07-244c-453e-aa97-4bcc7760cd79"), false, "Magnesium, Mg", "mg" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("00187677-4fd7-4640-bfa2-64f492ca0d88"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("01d61688-924e-4a3d-8e39-1caca84e0844"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("02868c37-7a2d-40af-bcd5-aa9ba05933c5"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("031d7115-a0b5-4663-8fae-cd2d54d5860f"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("08e0d1ce-5fb3-4780-a9e7-878a6da46ef4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("09a87134-6acd-433f-9f22-82949ad5878e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("09b254c2-6196-4d4a-bfb2-3acd9f5fd9f8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("09ec2628-6c48-42ac-b3c7-a6ce53a4686c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0a8f83a8-225b-4332-97da-1b39ff2eaf5d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0e1d39c8-500e-468a-8799-7497753a9186"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0e47721a-14b9-4174-a73d-5515cfbd00c6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0f3cfcbf-1eb2-411c-aec9-a779bced180c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("0fd0c57b-2b41-40a5-9c57-11085f19838c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("1143c769-af9d-4183-8907-c46df3d408a6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("13556c89-cc9c-4503-8254-8b519cfed2bb"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("145d0bbc-b6b9-4d6b-acd2-de7a5b382f58"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("15c90b30-4bf1-49b5-9149-ef09874044e9"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("180dd2ef-e910-4e7f-a8ab-cb41474d63b0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("195c204c-9a3a-48a9-a4ad-ae9fac6352fa"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("1a000942-9789-4ed0-8f68-84c168dc716b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("1a5ffa66-c3bc-4989-b863-e3469dcae1de"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("1d6e9df3-d6fd-47c0-82ce-57651eb1446a"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("213bfe86-6c2b-493f-a03f-a1d4fc58b2a0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("23b00007-ee3a-451d-9c8a-4bc238654f1d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("27a06531-a18d-43fa-8cb0-5120f346dfc1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("285681ee-6908-4887-9034-34dc2aab51d4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("298693dd-534c-4dcf-8755-f0df1483142e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("299364c5-1ca0-48aa-964f-149c39c75763"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2a91fd10-edd4-406c-b07b-8997dc5ee550"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2c139fd7-981a-4962-a4e5-e380aaa7e1d1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2c30679f-bf99-469c-9035-f9a973148b57"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("2f10c771-cd87-4579-8211-2099dacc598e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3048e404-f677-469c-9aef-ec5ad83158d2"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3126187d-5a90-4f94-b6a4-2bfa5a65eab7"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("31a5fc29-515e-4ac6-bc5c-018a5d5460dd"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("32cdc623-d7be-4f06-ab89-f071585d8c1c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("34a990a6-ecb2-4693-b6a2-7847dc5fe645"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("35159dfe-2b2e-4859-adb8-7d17c2df8ddf"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("35435ac0-5216-469c-abe1-bf47088650ea"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3bcaac8e-2033-4dfe-bc77-f3dac53d3adb"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3d6f2328-3da1-4f43-aa43-0bd4ff9356a8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3f8bd51b-531f-480c-a8d8-9172d721088b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("3fca3bf0-53ee-4b0a-be1d-25a70d3f5b48"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("40eb533c-d9b9-420e-b57f-0d3554c3b7ef"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("41880e48-1311-41c7-91d5-5eae5bb78e6b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("41e2b244-230a-4b54-b4ec-25db26ee7b4c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("42e2a0b2-cbbc-4f7c-a6a4-3bd73c53412f"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("43e78938-72f8-4234-bc1d-8722678ca10f"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("483fbbce-543e-494f-a71f-757c4683c7c3"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4a12bfd9-0157-45df-9a10-1f2a6e3811a1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4c9ecaa3-4382-4f38-acce-a39833120c7e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("4f2fd50b-0840-451d-a991-f174175bd456"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5a49107d-53b9-412d-8a80-40fe0397fc02"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5b1d3996-f0aa-40ce-afab-0535983f3f71"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5b52676a-741b-49d8-a842-e8b63fcae889"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5bcc8cdf-9ea9-477e-96dc-54e4fa8bfed3"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5ca3812b-d177-4a3e-8948-e45e1b5f2619"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5e1e5a94-47da-44e4-b1cb-2fec4d6cf81d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("5fa842ab-7781-4bf1-8065-8debfaa5b811"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("603875d4-a309-4e39-922c-37218fa1974c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("62fdf1a3-209b-4e74-9ed6-0a2da6df10c0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("657a8a9f-bcf2-48bd-ac29-f1efa7fb0701"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("66db2a18-bcff-4797-ae0d-65a8a3bc41a1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("6adcbb0e-f797-4116-877d-2ba61c7ec2c8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("6b597ae6-0dd2-4c15-91c7-d96be4767274"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("6fb27a04-781e-49d5-a0cb-c25d7c78e7df"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("711914de-5c17-4eb0-8749-b15912c6a634"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("71771f9e-ab46-42eb-9538-261a0ae70596"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7386602a-8239-4c3e-a354-5cc1c665fe66"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("78979ca6-2f2e-449d-901f-03dc233b48d4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7b93f187-5386-40b3-a053-aea6548e8eb5"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7e539245-c33e-4bc9-ab3e-09a3bce96a38"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7e581336-4f25-4866-8a8a-e22305a2e9e0"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("7e89fa22-068b-4439-8cf6-284fc943f4fe"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("82277d5b-19e9-4f3c-b2a5-2a7a001682bb"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("833d08a9-8775-4187-84d3-278efe75a5f1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("880790e8-858f-49a3-ac0d-6816a03f0a8d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("88a4ff58-bd22-4e32-b169-c6d6ef67d2ee"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8a8de098-e4af-4280-8121-172da7fa1653"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8b6a0565-530a-4ae4-8163-201c485620b7"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8c3aca94-7a61-4cad-b97b-87f7ce9b7c26"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8df11934-8272-474d-aedb-92ca6862384b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("8ef2c8a9-c00e-40c2-89f1-111a41c6c174"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("91347054-d516-4cfb-a876-e0293703f636"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("92904bea-e00f-48ae-9403-488b0f7b3bdb"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("9457e52c-76cc-4391-b0e7-28d028e7dc0d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("95ce08d0-d496-4f58-a704-9cc8ae00bae6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("96e690a3-c48b-4a78-8f1b-5b456d197ddd"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("9792947b-cd46-4d9e-b175-4ba09c460d0e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("986480b6-e2aa-489e-897c-81926320d4e5"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("9d4b0a55-5022-4559-ab70-e26b8bd8f533"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a1539430-c0d7-4c43-8c5d-4c2135b48e6e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a3d19f95-1ae7-4ac5-9cd1-c39adb1d9683"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a7242e2f-9f8b-463c-9060-1aae8b42e887"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("a8e864c0-6961-450c-a521-17ba7a39223e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("aa2f287c-cc85-4e96-8c6a-e1ca4e5ba4e6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("af9575b8-8158-46b0-8980-2bb3b03df6e9"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b0b77c9a-79c7-40f1-b7e4-72f34068288e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b1ca0be3-46ea-4fc4-9ea4-f6b5460eea17"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b263a17b-f130-4689-93fc-417b2306b920"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b3a9e840-d577-4527-bd87-288f8c5c9958"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b5454db4-5500-418e-8baf-4e698e54e715"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b7ddacdd-33d9-4ffd-bf63-df5bf5a19c23"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("b9af5c83-26ff-49ca-9057-6cb05da209b4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bc1ba0b5-0ca3-4096-80d9-563eb1e57d97"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bcdb9b56-8b3e-42ad-815e-2b7ee52ebc27"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("be1a8d9d-fd1e-4ebc-be95-abe5b1121d3d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("bfb22e75-b296-4b79-8fe7-aa1fa049732e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c050ffb9-f639-4dd6-b692-108a5ac19878"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c0b38407-970f-4690-be8c-6b9735eef4fd"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c4b978dd-9415-430b-8f96-37152b817447"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c53d74db-b6d0-4d04-a61e-cb87c3bdebb4"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c59d802f-0cef-410c-87f1-e64b9e68c072"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c5ea852b-65b3-43be-8b31-ab0bbabe6814"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c6507833-497d-4060-8db1-cbf638f5a803"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c79dfa14-77c6-483f-a7f0-26e442b9df28"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c8378c55-f327-4ff5-aa18-ca439a26b601"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c91397cf-b878-41e8-aad5-8ebc58af3857"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c952ffbd-bb70-4bb8-9bb3-5f64cc2a2a5a"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("c9b361bc-228e-475e-8188-3a4314802351"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cb760f8b-d0cf-4fce-9f9d-0c60167644ca"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cbf7cd31-2708-4956-b1c7-d12dcd691208"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cbfa7cdf-e781-4a8a-8b93-1fca326d7ea6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cc5582a4-d536-43bb-8ddf-8a4a44cb8c9a"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cc9f1df6-c625-4e1b-890b-3804d4a18d21"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("cd365c75-7129-4748-8af0-a8cbd505a0ec"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d17dd16e-4313-4fbe-b754-236d6844122f"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d226a906-793e-4687-bc45-aed98ddadf7d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d3198cc8-0dd0-4e72-acf0-256799619627"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d4d13698-7c99-4560-972f-8df8d1bf50b8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d5481a82-d13f-4c60-b454-243111f03a49"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("d68f7f64-0c63-4e0e-aa3a-93f97ec37b14"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("db803808-12a1-4e5f-b9dc-a932f5cb6ad6"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("dc62a44f-632a-4f56-adb9-51f8d0e6c896"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("de113fea-6505-4bbe-9c8e-5f44d974e26d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("df660f92-ea89-4a90-b7bf-2bf090b84dad"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e0ac6740-8942-44b5-931d-fb396631ee40"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e1232069-daf0-444b-9546-8728fd184bca"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e1511fd1-3de1-45ed-832a-ee5b4d61f983"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e18bfbeb-fc4e-4d87-84a2-e28f855db851"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e35581d9-2c00-4182-b305-487e76de5204"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e5366684-4dd5-4342-91fa-b9e7602ac195"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e5b847c7-2bff-46e6-8b2d-147b76bcf043"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("e6fd620d-1255-4cd2-b8e0-10b3f70c38b1"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ed279501-bbaf-40de-9d24-3de7e2738803"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ed3485f6-f55a-4cc7-b652-b99d2c82f64b"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("eebf87fe-53a1-482c-9ec2-01e084dd5d74"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("eec306a8-2f88-4d65-8c9b-51cd9dcd2d06"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("eef6bebc-dbc5-4c85-b668-8c05bb26a3e8"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f234bbd7-1fe1-4ca9-a0ba-4f58c6a26e7c"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f2bc3b0f-5fed-4037-aede-119755394488"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f350a432-e749-4a78-b4ef-a85ed7c6533e"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f71f0f17-00da-4a04-8942-c87a005f30c9"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f7362078-3adf-4cfd-accf-7ec71d59c353"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f79ed152-05d5-4ac9-9b1b-81f4a28d8698"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f8ad92bd-5123-4741-b0c0-e24f1bcb0888"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("f9fa052e-ff54-40a0-90cf-f34257baf5f2"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fb89788e-4fa2-4e4a-b211-74470af49b65"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fd0715b9-c807-4106-bd41-82ad6e59ae24"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("fdec4a36-3c31-43a8-b96b-22aa746b892d"));

            migrationBuilder.DeleteData(
                table: "Nutrients",
                keyColumn: "Id",
                keyValue: new Guid("ff56dd07-244c-453e-aa97-4bcc7760cd79"));
        }
    }
}
