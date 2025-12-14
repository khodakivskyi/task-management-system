# Code-First підхід для Task Management System

## Огляд

Ця папка містить реалізацію **Code-First** підходу для Entity Framework Core, який є альтернативою до **Database-First** (scaffolded) підходу з папки `EFModels`.

## Структура

```
CodeFirst/
├── Entities/              # Очищені entity класи без scaffolding annotations
│   ├── Category.cs
│   ├── Comment.cs
│   ├── EntityType.cs
│   ├── Favorite.cs
│   ├── Project.cs
│   ├── ProjectMember.cs
│   ├── ProjectRole.cs
│   ├── Status.cs
│   ├── Task.cs
│   ├── TaskAssignee.cs
│   ├── TaskHistory.cs
│   └── User.cs
├── Configurations/       # Fluent API конфігурації через IEntityTypeConfiguration<T>
│   ├── CategoryConfiguration.cs
│   ├── CommentConfiguration.cs
│   ├── EntityTypeConfiguration.cs
│   ├── FavoriteConfiguration.cs
│   ├── ProjectConfiguration.cs
│   ├── ProjectMemberConfiguration.cs
│   ├── ProjectRoleConfiguration.cs
│   ├── StatusConfiguration.cs
│   ├── TaskConfiguration.cs
│   ├── TaskAssigneeConfiguration.cs
│   ├── TaskHistoryConfiguration.cs
│   └── UserConfiguration.cs
├── TaskManagementCodeFirstDbContext.cs  # Новий DbContext з нуля
├── CodeFirstDemo.cs                     # Демонстрація використання
├── ProgramExample.cs                    # Приклад інтеграції в Program.cs
└── README.md                            # Ця документація
```

## Порівняння Database-First vs Code-First

### Database-First (Scaffolded) - `EFModels/`

**Характеристики:**
- Entity класи генеруються автоматично з існуючої бази даних
- Використовуються data annotations (`[Key]`, `[Column]`, `[ForeignKey]`, тощо)
- Містить багато scaffolding artifacts (коментарі, атрибути)
- DbContext генерується автоматично
- База даних є джерелом істини

**Приклад (BEFORE - згенерований):**
```csharp
[Table("Books")]
[Index("CategoryId", Name = "IX_Books_CategoryId")]
public partial class Book
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }
    
    [Column("Title")]
    [StringLength(200)]
    [Required]
    public string Title { get; set; } = null!;
    
    [ForeignKey("CategoryId")]
    [InverseProperty("Books")]
    public virtual Category? Category { get; set; }
}
```

### Code-First - `CodeFirst/`

**Характеристики:**
- Entity класи створюються вручну, чисті та прості
- Використовується Fluent API для конфігурації
- Окремі configuration класи через `IEntityTypeConfiguration<T>`
- DbContext створюється з нуля
- Код є джерелом істини, база даних генерується з міграцій

**Приклад (AFTER - очищений):**
```csharp
// Entity class - чистий, без annotations
public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public int? CategoryId { get; set; }
    
    // Navigation properties
    public Category? Category { get; set; }
}

// Configuration class - Fluent API
public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Title)
            .IsRequired()
            .HasMaxLength(200);
        builder.HasOne(b => b.Category)
            .WithMany(c => c.Books)
            .HasForeignKey(b => b.CategoryId);
    }
}
```

## Ключові відмінності

| Аспект | Database-First | Code-First |
|--------|---------------|------------|
| **Джерело істини** | База даних | Код (entity класи) |
| **Конфігурація** | Data annotations | Fluent API |
| **Структура** | Один файл на entity | Entity + Configuration класи |
| **Генерація** | Автоматична (scaffolding) | Вручну |
| **Міграції** | Не потрібні (БД вже існує) | Обов'язкові (генерують БД) |
| **Гнучкість** | Обмежена (залежить від БД) | Висока (повний контроль) |
| **Чистота коду** | Багато scaffolding artifacts | Чистий код |
| **Підтримка** | Складніша (синхронізація з БД) | Простіша (все в коді) |

## Переваги Code-First підходу

1. **Чистий код**: Entity класи без scaffolding annotations
2. **Розділення відповідальності**: Entity та Configuration окремо
3. **Версіонування**: Міграції дозволяють відстежувати зміни схеми
4. **Гнучкість**: Легко додавати нові властивості та entities
5. **Тестування**: Легше створювати тестові бази даних
6. **Контроль**: Повний контроль над структурою бази даних

## Недоліки Code-First підходу

1. **Більше коду**: Потрібно писати configuration класи
2. **Міграції**: Потрібно керувати міграціями вручну
3. **Складність**: Більше кроків для початку роботи

## Коли використовувати Code-First?

✅ **Використовуйте Code-First, коли:**
- Створюєте новий проєкт з нуля
- Потрібен повний контроль над структурою БД
- Хочете використовувати міграції для версіонування
- Важлива чистота та організація коду
- Плануєте часто змінювати схему БД

❌ **Використовуйте Database-First, коли:**
- Працюєте з існуючою БД
- БД часто змінюється іншими командами/інструментами
- Потрібна швидка інтеграція з існуючою БД
- Не плануєте змінювати структуру БД

## Використання

### 1. Реєстрація DbContext в Program.cs

```csharp
builder.Services.AddDbContext<TaskManagementCodeFirstDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<CodeFirstDemo>();
```

### 2. Використання в коді

```csharp
public class MyService
{
    private readonly TaskManagementCodeFirstDbContext _context;

    public MyService(TaskManagementCodeFirstDbContext context)
    {
        _context = context;
    }

    public async Task<List<Task>> GetTasksAsync()
    {
        return await _context.Tasks
            .Include(t => t.Owner)
            .Include(t => t.Status)
            .ToListAsync();
    }
}
```

### 3. Створення міграцій

```bash
# Створити міграцію
dotnet ef migrations add InitialCreate --context TaskManagementCodeFirstDbContext

# Застосувати міграцію
dotnet ef database update --context TaskManagementCodeFirstDbContext
```

## Структура Configuration класів

Кожен configuration клас реалізує `IEntityTypeConfiguration<T>` та містить:

1. **Primary Key**: `builder.HasKey(e => e.Id)`
2. **Properties**: Налаштування властивостей (максимальна довжина, обов'язковість, типи)
3. **Indexes**: Створення індексів (простих та композитних)
4. **Relationships**: Налаштування відносин (one-to-many, many-to-many)
5. **Table name**: Явне вказання імені таблиці
6. **Default values**: Значення за замовчуванням
7. **Delete behavior**: Поведінка при видаленні (Cascade, Restrict, SetNull)

## Висновок

Code-First підхід надає більше контролю та гнучкості для розробки, але вимагає більше коду та уваги до міграцій. Database-First підхід швидший для інтеграції з існуючою БД, але менш гнучкий для змін.

Обидва підходи можуть співіснувати в одному проєкті, якщо потрібно працювати з різними контекстами або поступово мігрувати з Database-First на Code-First.

