using MediatR;
using Microsoft.AspNetCore.Mvc;
using VehicleResale.Application.Commands;
using VehicleResale.Application.DTOs;
using VehicleResale.Application.Queries;

namespace VehicleResale.API.Controllers
{
    /// <summary>
    /// Controller responsável pelo gerenciamento de veículos
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class VehiclesController(IMediator mediator) : ControllerBase
    {
        /// <summary>
        /// Cadastra um novo veículo para venda
        /// </summary>
        /// <param name="dto">Dados do veículo</param>
        /// <returns>Veículo cadastrado</returns>
        [HttpPost]
        [ProducesResponseType(typeof(VehicleDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto dto)
        {
            var command = new CreateVehicleCommand(dto);
            var result = await mediator.Send(command);
            return CreatedAtAction(nameof(GetAvailableVehicles), new { id = result.Id }, result);
        }

        /// <summary>
        /// Atualiza os dados de um veículo
        /// </summary>
        /// <param name="id">ID do veículo</param>
        /// <param name="dto">Dados atualizados do veículo</param>
        /// <returns>Veículo atualizado</returns>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(VehicleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateVehicle(Guid id, [FromBody] UpdateVehicleDto dto)
        {
            if (id != dto.Id)
                return BadRequest("ID mismatch");

            var command = new UpdateVehicleCommand(dto);
            var result = await mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os veículos disponíveis para venda (ordenados por preço)
        /// </summary>
        /// <returns>Lista de veículos disponíveis</returns>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<VehicleDto>), 200)]
        public async Task<IActionResult> GetAvailableVehicles()
        {
            var query = new GetAvailableVehiclesQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Lista todos os veículos vendidos (ordenados por preço)
        /// </summary>
        /// <returns>Lista de veículos vendidos</returns>
        [HttpGet("sold")]
        [ProducesResponseType(typeof(IEnumerable<VehicleSaleDto>), 200)]
        public async Task<IActionResult> GetSoldVehicles()
        {
            var query = new GetSoldVehiclesQuery();
            var result = await mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Registra a venda de um veículo
        /// </summary>
        /// <param name="dto">Dados da venda</param>
        /// <returns>Informações da venda registrada</returns>
        [HttpPost("sale")]
        [ProducesResponseType(typeof(VehicleSaleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RegisterSale([FromBody] RegisterSaleDto dto)
        {
            var command = new RegisterSaleCommand(dto);
            var result = await mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Webhook para atualização do status de pagamento
        /// </summary>
        /// <param name="dto">Dados do webhook de pagamento</param>
        /// <returns>Status da operação</returns>
        [HttpPost("payment-webhook")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymentWebhookDto dto)
        {
            var command = new UpdatePaymentStatusCommand(dto);
            var success = await mediator.Send(command);
            
            if (success)
                return Ok(new { message = "Status de pagamento atualizado com sucesso!" });
            
            return NotFound(new { message = "Código de pagamento não encontrado" });
        }
    }
}