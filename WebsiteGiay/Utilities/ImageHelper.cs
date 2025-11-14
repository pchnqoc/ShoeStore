namespace WebsiteGiay.Utilities
{
    public static class ImageHelper
    {
        public static string? ResolveImagePath(string? hinhAnh)
        {
            if (string.IsNullOrWhiteSpace(hinhAnh))
            {
                return null;
            }

            var path = hinhAnh.Trim();
            if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
            {
                return path;
            }

            path = path.Replace("\\", "/");
            if (!path.StartsWith("images/", StringComparison.OrdinalIgnoreCase) &&
                !path.StartsWith("/images/", StringComparison.OrdinalIgnoreCase))
            {
                path = $"images/{path.TrimStart('/')}";
            }
            else
            {
                path = path.TrimStart('/');
            }

            return $"/{path}";
        }
    }
}


