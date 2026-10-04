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

 

        }

        public static void SeedRoles(RoleManager<IdentityRole> roleManager, List<string> roles) {

            foreach (string roleName in roles) {
                //it checks such role does not exist in the database 
                if (!roleManager.RoleExistsAsync(roleName).Result) {
                    IdentityRole role = new IdentityRole();
                    role.Name = roleName;
                    role.NormalizedName = roleName;
                    IdentityResult roleResult = roleManager.CreateAsync(role).Result;
                }
            }

        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager, List<string> roles) {
            //first, it checks the user does not already exist in the DB
            if (userManager.FindByNameAsync("elena@uclm.es").Result == null) {
                ApplicationUser user = new ApplicationUser("1", "Elena", "Navarro Martínez", "elena@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "Password1234%");
                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //administrator role
                    userManager.AddToRoleAsync(user, roles[0]).Wait();
                }
            }


            if (userManager.FindByNameAsync("peter@uclm.es").Result == null) {
                //A customer class has been defined because it has different attributes (purchase, rental, etc.)
                ApplicationUser user = new ApplicationUser("3", "Peter", "Jackson", "peter@uclm.es");
                user.EmailConfirmed = true;

                var result = userManager.CreateAsync(user, "OtherPass12$");

                result.Wait();

                if (result.IsCompletedSuccessfully) {
                    //customer role
                    userManager.AddToRoleAsync(user, roles[2]).Wait();

                }
            }

        }


        // --- NUEVO MÉTODO PARA CREAR ACCESORIOS ---
        public static void SeedAccesorios(ApplicationDbContext dbContext) {
            // Comprobamos si ya existen accesorios en la BD para evitar duplicados
            if (!dbContext.Accesorios.Any()) {

                // 1. Crear Accesorios asociados con sus atributos (Nombre, Categoria, Compatibilidad, CantidadDisponible, Precio)
                var accesorio1 = new Accesorio {
                    Nombre = "Boquilla de Acero Endurecido 0.4mm",
                    Categoria = CategoriaAccesorio.Boquillas,
                    Compatibilidad = "Creality Ender 3 / Prusa MK3",
                    CantidadDisponible = 25,
                    Precio = 12.50m
                };

                var accesorio2 = new Accesorio {
                    Nombre = "Base Magnética PEI Texturizada",
                    Categoria = CategoriaAccesorio.BasesImpresion, // Corregido: BasesImpresion con una sola 'b'
                    Compatibilidad = "Bambu Lab X1 / P1P",
                    CantidadDisponible = 10,
                    Precio = 29.99m
                };

                var accesorio3 = new Accesorio {
                    Nombre = "Kit de Limpieza de Boquillas y Pinzas",
                    Categoria = CategoriaAccesorio.KitsLimpieza,
                    Compatibilidad = "Universal",
                    CantidadDisponible = 50,
                    Precio = 8.75m
                };

                dbContext.Accesorios.AddRange(accesorio1, accesorio2, accesorio3);

                // 2. Guardar cambios en la base de datos
                dbContext.SaveChanges();
            }
        }


    }
}