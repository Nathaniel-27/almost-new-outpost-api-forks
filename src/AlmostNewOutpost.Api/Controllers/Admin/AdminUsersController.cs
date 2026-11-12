using AlmostNewOutpost.Api.Data;
using AlmostNewOutpost.Api.Models.Entities;
using AlmostNewOutpost.Api.Models.Requests;
using AlmostNewOutpost.Api.Models.Responses;
using AlmostNewOutpost.Api.Models.Rows;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;



namespace AlmostNewOutpost.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/users/")]
    public class AdminUsersController(OutpostDbContext db) : ControllerBase
    {
        private readonly OutpostDbContext _db = db;

        [HttpGet]
        public async Task<ActionResult<List<AdminUserDto>>> GetAll([FromQuery] bool includeArchived = false)
        {
            List<AdminUserDto> users = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where includeArchived || user.ArchivedAtUtc == null
                orderby user.Username
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                }).ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdminUserDto>> GetById([FromRoute] int id)
        {
            AdminUserDto? adminUser = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where user.UserId == id
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                })
                .FirstOrDefaultAsync();

            if (adminUser == null)
            {
                return Problem(title: "User not found.", statusCode: StatusCodes.Status404NotFound);

            }
            return Ok(adminUser);

        }



        private async Task<AdminUserDto?> FindUserAsync(int id)
        {

            AdminUserDto? adminUser = await (
                from user in _db.Users
                join role in _db.Roles on user.RoleId equals role.RoleId
                where user.UserId == id
                select new AdminUserDto
                {
                    Id = user.UserId,
                    Username = user.Username,
                    RoleId = role.RoleId,
                    Role = role.RoleName,
                    ArchivedAtUtc = user.ArchivedAtUtc,
                })
                .FirstOrDefaultAsync();

            return adminUser;
        }

        IPasswordHasher<UserRow> _passwordHasher = new PasswordHasher<UserRow>();

        [HttpPost]
        public async Task<ActionResult<AdminUserDto>> Create(CreateUserRequest request)
        {
            string username = request.Username.Trim();
            bool usernameTaken = await _db.Users.AnyAsync(user => user.Username == username);
            if (usernameTaken)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            bool roleExists = await _db.Roles.AnyAsync(role => role.RoleId == request.RoleId);
            if (!roleExists)
            {
                ModelState.AddModelError(nameof(CreateUserRequest.RoleId), "Choose a role that exists.");
                return ValidationProblem(ModelState);
            }

            string passwordHash = _passwordHasher.HashPassword(new UserRow(), request.Password);
            User user = new User { Username = username, PasswordHash = passwordHash, RoleId = request.RoleId!.Value };
            _db.Users.Add(user);

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateException exception) when (DatabaseErrors.GetErrorNumber(exception) == DatabaseErrors.DuplicateKey)
            {
                return Problem(title: "That username is already taken.", statusCode: StatusCodes.Status409Conflict);
            }

            AdminUserDto? created = await FindUserAsync(user.UserId);
            return CreatedAtAction(nameof(GetById), new { id = user.UserId }, created);



        }

    }

}
























