namespace Basic.WebAPI.Helpers
{
    public static class CommonError
    {
        public static string ServerSideError => "Error while updating your data, please try again later.";
    }


    public static class AccountError
    {
        public static string InvalidEmailOrPassword => "Invalid Email/Password.";
        public static string UserAlreadyExist => "User exists.";
    }

    public static class DoctorProfileError
    {
        public static string DoctorNotFound => "Doctor not found.";
        public static string DoctorNotUpdated => "Can't update the doctor profile.";
    }
}