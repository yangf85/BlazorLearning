namespace BlazorLearning.Shared.Dtos
{
    public class AssignRoleRequest
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    public class UnassignRoleRequest
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    public class ReplaceUserRolesRequest
    {
        public int UserId { get; set; }
        public List<int> RoleIds { get; set; } = new();
    }

    public class UserRoleResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<RoleInfo> Roles { get; set; } = new();
    }

    public class RoleUserResponse
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        public List<UserInfo> Users { get; set; } = new();
    }

    public class UserRoleDetailResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int AssignedBy { get; set; }
        public string AssignedByUsername { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RoleInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime AssignedAt { get; set; }
        public bool IsActive { get; set; }
    }
}