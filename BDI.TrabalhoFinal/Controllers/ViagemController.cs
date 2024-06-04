using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BDI.TrabalhoFinal.Data;
using BDI.TrabalhoFinal.Models;

namespace BDI.TrabalhoFinal.Controllers
{
    public class ViagemController : Controller
    {
        private readonly BancoDeDados _context;

        public ViagemController(BancoDeDados context)
        {
            _context = context;
        }

        // GET: Viagem
        public async Task<IActionResult> Index()
        {
            var bancoDeDados = _context.Viagens
                .Include(v => v.Veiculo)
                .Include(v => v.Passageiro)
                .Include(v => v.Motorista);

            return View(await bancoDeDados.ToListAsync());
        }

        // GET: Viagem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens
                .Include(v => v.Veiculo)
                .Include(v => v.Passageiro)
                .Include(v => v.Motorista)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viagem == null)
            {
                return NotFound();
            }

            return View(viagem);
        }

        // GET: Viagem/Create
        public IActionResult Create()
        {
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Viagem viagem)
        {
            //if (ModelState.IsValid)
            //{
                var motorista = await _context.Motoristas.FirstOrDefaultAsync(m => m.CPF.Equals(viagem.CpfMotorista));
                viagem.Motorista = motorista;
                var passageiro = await _context.Passageiros.FirstOrDefaultAsync(m => m.CPF.Equals(viagem.CpfPassageiro));
                viagem.Passageiro = passageiro;
                _context.Add(viagem);
                await _context.SaveChangesAsync();
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", viagem.VeiculoId);
                return RedirectToAction(nameof(Index));
            //}
            //return View(viagem);
        }

        // GET: Viagem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens.FindAsync(id);
            if (viagem == null)
            {
                return NotFound();
            }
            ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", viagem.VeiculoId);
            return View(viagem);
        }

        // POST: Viagem/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Viagem viagem)
        {
            if (id != viagem.Id)
            {
                return NotFound();
            }

            //if (ModelState.IsValid)
            //{
                try
                {
                    _context.Update(viagem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViagemExists(viagem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                ViewData["VeiculoId"] = new SelectList(_context.Veiculos, "Id", "Id", viagem.VeiculoId);
                return RedirectToAction(nameof(Index));
            //}
            //return View(viagem);
        }

        // GET: Viagem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var viagem = await _context.Viagens
                .Include(v => v.Veiculo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (viagem == null)
            {
                return NotFound();
            }

            return View(viagem);
        }

        // POST: Viagem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var viagem = await _context.Viagens.FindAsync(id);
            if (viagem != null)
            {
                _context.Viagens.Remove(viagem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ViagemExists(int id)
        {
            return _context.Viagens.Any(e => e.Id == id);
        }

        private async Task<IActionResult> listaViagem(string brand) 
        {
            var date = new DateTime(2024, 1, 3);
            var startTime = new DateTime(2024, 1, 3, 21, 0, 0);
            var endTime = new DateTime(2024, 1, 3, 21, 59, 59);

            var trips = _context.Viagens
            .Include(t => t.Veiculo)
            .Include(t => t.Motorista)
            .Include(t => t.Passageiro)
            .Where(t => t.Veiculo.Marca == brand && t.StartTime >= startTime && t.EndTime <= endTime)
            .Select(t => new
            {
                t.Veiculo.Marca,
                t.Veiculo.Placa,
                t.Origem,
                t.Destino,
                DriverName = t.Motorista.Nome,
                PassengerName = t.Passageiro.Nome
            })
            .ToList();

            ViewBag.Brand = brand;
            return View(trips);
        }

        private async Task<IActionResult> listaMaioresFaturamentos(int ano, int mes)
        {
            var topFaturamentos = await _context.Viagens
                .Where(r => r.Date.Year == ano &&  r.Date.Month == mes)
                .OrderByDescending(r => r.Valor)
                .Take(20)
                .ToList();

            ViewBag.Year = ano;
            ViewBag.Month = month;
            return View(topFaturamentos);
        }
    }
}
