using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Gauniv.WebServer.Data;
using Gauniv.WebServer.Dtos;
using AutoMapper;

namespace Gauniv.WebServer.Api
{
    [Route("api/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        /// 📌 **GET /api/categories** - Retourne la liste des catégories
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            var categoriesDto = _mapper.Map<List<CategoryDto>>(categories);
            return Ok(categoriesDto);
        }

        /// 📌 **POST /api/categories** - Ajouter une catégorie
        [HttpPost]
        public async Task<IActionResult> AddCategory([FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
                return BadRequest("Le nom de la catégorie est requis.");

            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, _mapper.Map<CategoryDto>(category));
        }

        /// 📌 **PUT /api/categories/{id}** - Modifier une catégorie
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null || string.IsNullOrWhiteSpace(categoryDto.Name))
                return BadRequest("Le nom de la catégorie est requis.");

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound($"La catégorie avec l'ID {id} n'existe pas.");

            category.Name = categoryDto.Name;
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CategoryDto>(category));
        }

        /// 📌 **DELETE /api/categories/{id}** - Supprimer une catégorie
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound($"La catégorie avec l'ID {id} n'existe pas.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
