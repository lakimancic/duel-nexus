namespace Backend.Utils.WebApi;

public class ObjectNotFoundException(string message) : Exception(message) {}
public class BadRequestException(string message) : Exception(message) {}
public class ConflictObjectException(string message) : Exception(message) {}
