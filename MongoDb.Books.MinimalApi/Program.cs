using MongoDb.Books.Main;
using MongoDB.Bson;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IMongoDbDataService, MongoDbDataService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

//GET ALL
app.MapGet("/books", (QueryGetAllParameters parameters, IMongoDbDataService dataService) =>
{
    var limit = parameters?.Limit;
    var offset = parameters?.Offset;

    var books = (limit.HasValue && offset.HasValue) ?
        dataService.Get(parameters.Limit.Value, parameters.Offset.Value) :
        dataService.Get();

    return Results.Ok(books);
}).WithName("GetBooks");

//GET BY ID
app.MapGet("/books/{id}", async (ObjectId id, IMongoDbDataService dataService) =>
    await dataService.GetByIdAsync(id, cancellationToken: default)
        is Book book ? Results.Ok(book) : Results.NotFound())
    .WithName("GetBookById");

//POST
app.MapPost("/books", async (Book book, IMongoDbDataService dataService) =>
    await dataService.CreateAsync(book, cancellationToken: default)
        is Book newBook ? Results.CreatedAtRoute("GetBookById", new { id = newBook._id }, newBook) : Results.BadRequest())
    .WithName("CreateBook");

//PUT
app.MapPut("/books/{id}", async (ObjectId id, Book book, IMongoDbDataService dataService) =>
    await dataService.UpdateAsync(id, book, cancellationToken: default)
        is Book updatedBook ? Results.Ok(updatedBook) : Results.BadRequest())
    .WithName("UpdateBook");

//DELETE
app.MapDelete("/books/{id}", async (ObjectId id, IMongoDbDataService dataService) =>
{
    await dataService.DeleteAsync(id, cancellationToken: default);
    return Results.Ok();
}).WithName("DeleteBook");

//POST SEARCH
app.MapPost("/books/search", async (SearchCriteria criteria, IMongoDbDataService dataService) =>
    await dataService.SearchAsync(criteria, cancellationToken: default)
).WithName("SearchBooks");

app.Run();

public record QueryGetAllParameters(int? Limit, int? Offset)
{
    public static ValueTask<QueryGetAllParameters?> BindAsync(HttpContext context)
        => ValueTask.FromResult<QueryGetAllParameters?>(
            new(
            Limit: int.TryParse(context.Request.Query["limit"], out var limit) ? limit : null,
            Offset: int.TryParse(context.Request.Query["offset"], out var offset) ? offset : null));
}