namespace AppForSEII.API.Data {
    public class SeedData {
        public static void Initialize(ApplicationDbContext dbContext, IServiceProvider serviceProvider, ILogger logger) {
            List<string> rolesNames = new List<string> { "Administrator", "Employee", "Customer" };

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            try {
                SeedRoles(roleManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the roles in the Database.");
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            try {
                SeedUsers(userManager, rolesNames);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Users in the Database.");
            }

            // --- NUEVO: Seeding de Piezas3D y Materiales ---
            try {
                SeedPiezas3D(dbContext);
            }
            catch (Exception ex) {
                logger.LogError(ex, "An error occurred seeding the Piezas3D in the Database.");
            }
        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {
            foreach (string roleName in roles) {
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }

            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    userManager.AddToRoleAsync(user, roles[2]).Wait();
                }
            }
        }

        // --- NUEVO MÉTODO PARA CREAR PIEZAS 3D Y MATERIALES ---
        public static void SeedPiezas3D(ApplicationDbContext dbContext) {
            // Comprobamos si ya existen piezas en la BD para evitar duplicados
            if (!dbContext.Piezas3D.Any()) {

                // 1. Crear Materiales requeridos según el diagrama
                var matPLA = new Material {
                    Nombre = "PLA Premium",
                    PrecioPorGramo = 0.03m,
                    StockGramos = 5000m
                };

                var matABS = new Material {
                    Nombre = "ABS Resistente",
                    PrecioPorGramo = 0.04m,
                    StockGramos = 3000m
                };

                dbContext.Materiales.AddRange(matPLA, matABS);

                // 2. Crear Piezas3D asociándolas con sus Materiales y Categorías
                var pieza1 = new Pieza3D {
                    Nombre = "Soporte para Auriculares",
                    Peso = 120.5m,
                    Categoria = CategoriaPieza.HerramientasYAccesorios,
                    MaterialesValidos = new HashSet<Material> { matPLA, matABS }
                };

                var pieza2 = new Pieza3D {
                    Nombre = "Figura Dragón Articulado",
                    Peso = 85.0m,
                    Categoria = CategoriaPieza.Decoracion,
                    MaterialesValidos = new HashSet<Material> { matPLA }
                };

                var pieza3 = new Pieza3D {
                    Nombre = "Engranaje Recambio M3",
                    Peso = 15.2m,
                    Categoria = CategoriaPieza.Repuestos,
                    MaterialesValidos = new HashSet<Material> { matABS }
                };

                dbContext.Piezas3D.AddRange(pieza1, pieza2, pieza3);

                // 3. Guardar cambios en la base de datos
                dbContext.SaveChanges();
            }
        }
    }
}