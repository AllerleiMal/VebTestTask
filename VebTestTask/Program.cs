using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using VebTestTask.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<UserContext>(options => options.UseSqlServer(connection));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Users CRUD API",
        Description = "An ASP.NET Core Web API for managing users and their roles",
    });
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
// builder.Services.AddSingleton<SieveProcessor>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();




// const int InitialNumberOfUsers = 100;
//             const int InitialNumberOfRoles = 4;
//             
//             migrationBuilder.InsertData(
//                 table: "Roles",
//                 columns: new []{nameof(Role.Name)},
//                 values: new object[,]
//                 {
//                     {"User"},
//                     {"Admin"},
//                     {"Support"},
//                     {"SuperAdmin"}
//                 }
//             );
//             migrationBuilder.InsertData(
//                 table: "Users",
//                 columns: new []{nameof(User.Name), nameof(User.Age), nameof(User.Email)},
//                 values: new object[,]
//                 { 
//                     {"Ora",93,"augue.porttitor@outlook.ca"}, 
//                     {"Dawn",15,"et.pede@outlook.ca"},
//                     {"Shelly",39,"placerat@google.edu"},
//                     {"Judith",73,"sit.amet@yahoo.org"},
//                     {"Camille",91,"tincidunt@aol.ca"},
//                     {"Jolie",53,"faucibus.orci@yahoo.com"},
//                     {"Priscilla",57,"pellentesque.a.facilisis@outlook.org"},
//                     {"Kamal",4,"nam.porttitor@yahoo.net"},
//                     {"Justine",60,"convallis.est.vitae@icloud.com"},
//                     {"Fleur",59,"sem.semper.erat@aol.edu"},
//                     {"Raymond",26,"volutpat.nulla.facilisis@protonmail.org"},
//                     {"Liberty",66,"dis@google.ca"},
//                     {"Lacy",51,"tristique.pharetra.quisque@google.com"},
//                     {"Vaughan",30,"eleifend.cras.sed@aol.couk"},
//                     {"Iliana",73,"varius.nam@aol.org"},
//                     {"Tallulah",13,"sit.amet@hotmail.edu"},
//                     {"Jemima",11,"augue@google.edu"},
//                     {"Arsenio",2,"pede@yahoo.edu"},
//                     {"Joan",70,"faucibus.leo.in@icloud.edu"},
//                     {"Amber",78,"enim.commodo@google.edu"},
//                     {"Thor",83,"non.feugiat.nec@outlook.edu"},
//                     {"Nichole",58,"convallis.ligula.donec@yahoo.couk"}, 
//                     {"Athena",2,"ut.quam@google.org"},
//                     {"Rosalyn",87,"risus.varius@yahoo.couk"},
//                     {"Laurel",7,"nascetur@yahoo.com"},
//                     {"Rina",70,"lorem.vehicula@hotmail.ca"},
//                     {"Benjamin",31,"parturient.montes.nascetur@protonmail.couk"},
//                     {"Preston",18,"scelerisque.neque.sed@aol.ca"},
//                     {"Cruz",65,"lectus@icloud.edu"},
//                     {"Glenna",49,"tellus.nunc.lectus@aol.net"},
//                     {"Barrett",49,"aliquam.gravida@hotmail.org"},
//                     {"Sopoline",34,"ante.maecenas@outlook.org"},
//                     {"Ruth",19,"consectetuer.mauris@outlook.com"},
//                     {"Jacob",16,"sagittis.semper.nam@icloud.couk"},
//                     {"Hamilton",26,"mauris.suspendisse@yahoo.edu"},
//                     {"Blake",50,"et.ultrices@yahoo.couk"},
//                     {"Nell",17,"donec.nibh@aol.ca"},
//                     {"Amal",75,"auctor@hotmail.couk"},
//                     {"Kiayada",12,"in.dolor.fusce@google.ca"},
//                     {"Kaseem",20,"nunc.ac.sem@outlook.couk"},
//                     {"Megan",16,"erat.volutpat@yahoo.edu"},
//                     {"Dai",43,"quam.a@hotmail.net"},
//                     {"Jenette",18,"tincidunt.congue@protonmail.com"},
//                     {"Mufutau",67,"dolor@outlook.net"},
//                     {"Ferris",14,"ornare.lectus@outlook.ca"},
//                     {"Cameron",7,"dui.cum.sociis@yahoo.com"},
//                     {"Roth",94,"id.sapien@yahoo.org"},
//                     {"Portia",10,"erat@outlook.net"},
//                     {"Raja",58,"placerat.eget@yahoo.ca"},
//                     {"Chaim",83,"tortor@google.edu"},
//                     {"Brian",41,"blandit.nam.nulla@outlook.ca"},
//                     {"Alana",56,"justo.nec@google.ca"},
//                     {"Rigel",47,"justo.sit.amet@google.org"},
//                     {"Natalie",26,"iaculis.quis.pede@protonmail.com"},
//                     {"Iris",3,"rutrum.magna@google.ca"},
//                     {"Hop",68,"sem.ut@hotmail.org"},
//                     {"Gil",77,"libero@icloud.com"},
//                     {"Brielle",80,"ante.dictum.cursus@yahoo.couk"},
//                     {"Tana",21,"quisque.porttitor@google.edu"},
//                     {"Lucy",14,"velit.cras.lorem@hotmail.org"},
//                     {"Jonah",50,"leo.vivamus.nibh@yahoo.net"},
//                     {"Dylan",29,"odio.auctor.vitae@google.com"},
//                     {"Briar",72,"fringilla@hotmail.net"},
//                     {"Ciara",12,"sed@hotmail.com"},
//                     {"Sandra",80,"suscipit.est@google.ca"},
//                     {"Josephine",80,"imperdiet@outlook.edu"},
//                     {"Philip",28,"rhoncus.donec.est@aol.org"},
//                     {"Heidi",79,"nibh.lacinia@google.org"},
//                     {"Whoopi",26,"dolor.sit.amet@yahoo.couk"},
//                     {"Gregory",47,"consectetuer.cursus@icloud.com"},
//                     {"Jeanette",80,"dolor.dapibus.gravida@protonmail.edu"},
//                     {"Jasmine",22,"montes.nascetur@icloud.com"},
//                     {"Leah",10,"vitae.mauris.sit@hotmail.ca"},
//                     {"Ira",24,"ac.feugiat.non@protonmail.com"},
//                     {"Guinevere",48,"tellus.imperdiet@yahoo.ca"},
//                     {"Ronan",27,"semper.pretium@aol.ca"},
//                     {"Kaye",47,"gravida.praesent@protonmail.com"},
//                     {"Garrett",89,"libero.mauris@hotmail.com"},
//                     {"Cadman",73,"facilisis.vitae.orci@protonmail.edu"},
//                     {"Nicole",9,"cubilia.curae@aol.com"},
//                     {"Brittany",4,"interdum.curabitur@icloud.com"},
//                     {"Hashim",28,"ut@hotmail.ca"},
//                     {"Derek",59,"justo.eu@yahoo.org"},
//                     {"Veronica",30,"blandit.nam@protonmail.couk"},
//                     {"Kiona",72,"feugiat@google.ca"},
//                     {"Jenette",43,"volutpat.nulla.dignissim@hotmail.org"},
//                     {"Lana",51,"orci.tincidunt@icloud.net"},
//                     {"Shelby",24,"tempor@hotmail.ca"},
//                     {"Paula",86,"convallis.ligula.donec@hotmail.org"},
//                     {"Keegan",45,"morbi.accumsan@protonmail.edu"},
//                     {"Nyssa",45,"pellentesque.ut@yahoo.couk"},
//                     {"Callie",89,"et.nunc.quisque@hotmail.org"},
//                     {"Cynthia",71,"non.nisi.aenean@hotmail.org"},
//                     {"Penelope",13,"phasellus.dapibus@hotmail.edu"},
//                     {"Kenneth",0,"elit.fermentum@protonmail.com"},
//                     {"Barclay",84,"malesuada.vel@outlook.com"},
//                     {"Cynthia",35,"lorem.vitae@yahoo.edu"},
//                     {"Jolie",16,"amet.ultricies@aol.ca"},
//                     {"Raja",56,"pellentesque.ultricies@icloud.org"},
//                     {"Lael",75,"primis.in@google.edu"}
//                 }
//             );
//             
//             var insertedUserRoles = new object[InitialNumberOfUsers + 30, 2];
//             {
//                 Random random = new Random();
//                 for (int i = 1; i <= InitialNumberOfUsers; i++)
//                 {
//                     insertedUserRoles[i - 1, 0] = i;
//                     insertedUserRoles[i - 1, 1] = random.Next(0, InitialNumberOfRoles + 1);
//                 }
//
//                 for (int i = 0; i < 30; ++i)
//                 {
//                     int randomUser = random.Next(1, InitialNumberOfUsers + 1);
//                     int randomRole = random.Next(InitialNumberOfRoles + 1);
//                     while ((int)insertedUserRoles[randomUser - 1, 1] == randomRole)
//                     {
//                         randomRole = random.Next(InitialNumberOfRoles + 1);
//                     }
//                     insertedUserRoles[InitialNumberOfUsers + i, 0] = randomUser;
//                     insertedUserRoles[InitialNumberOfUsers + i, 1] = randomRole;
//                 }
//             }
//             
//             migrationBuilder.InsertData(
//                 table: "RoleUser",
//                 columns: new []{"UsersId", "RolesId"},
//                 values: insertedUserRoles
//             );