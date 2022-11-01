namespace BookWebAPI.Common
{
    public class CustomExceptions : Exception
    {
        public CustomExceptions(string message)
            : base(message)
        {
        }
        public CustomExceptions()
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

        public class UnauthorizeException : CustomExceptions
        {
            public UnauthorizeException(string message) 
                : base(message)
            {
            }

            public UnauthorizeException()
            {
            }
        }

        public class ExistsException : CustomExceptions
        {
            public ExistsException(string message)
                : base(message)
            {
            }
        }
    }
}
