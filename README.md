# Love.Net.EF.AutoHistory
A plugin for Microsoft.EntityFrameworkCore to support automatically recording data changes history.

# How to Use

See more [HowToUse](https://github.com/lovedotnet/Love.Net.EF.AutoHistory/tree/master/src/HowToUse)

## Install Love.Net.EF.AutoHistory

To install Love.Net.EF.AutoHistory, run the following command in the Package Manager Console

`PM> Install-Package Love.Net.EF.AutoHistory`

## Enable auto history

To enable auto history functionality, need to two steps

1. `using Microsoft.EntityFrameworkCore;` in your DbContext.
2. Override the OnModelCreating method, as following:

	```csharp
	protected override void OnModelCreating(ModelBuilder modelBuilder) {
		base.OnModelCreating(modelBuilder);

		// enable auto history functionality.
		modelBuilder.EnableAutoHistory();
	}
	```

3. Call `_context.EnsureAutoHistory();` before `await _context.SaveChangesAsync();`

	```csharp
	[HttpPost]
	public async Task<int> Post([FromBody]Agenda agenda) {
		_context.Agenda.Add(agenda);

		// ensure auto history
		_context.EnsureAutoHistory();

		return await _context.SaveChangesAsync();
	}
	```

# Extensions

The **better way** to `ensure auto history` is to override the DbContext `SaveChangesAsync()` and `SaveChanges()` methods.