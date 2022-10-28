namespace BookWebAPI.Common
{
    public class CustomExceptions : Exception
    {
        public CustomExceptions(string message)
            : base(message)
        {
        }

        public class BadRequestException : CustomExceptions
        {
            public BadRequestException(string message) 
                : base(message)
            {
            }
        }

        public class NotFoundException : CustomExceptions
        {
            public NotFoundException(string message)
                : base(message)
            {
            }
        }
    }
}
