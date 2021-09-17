using DesafioJogos.Exceptions;
using DesafioJogos.InputModel;
using DesafioJogos.Services;
using DesafioJogos.ViewModel;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DesafioJogos.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
      
    {

        private readonly IJogoService _jogoService;

        public JogosController(IJogoService jogoService)
        {
            _jogoService = jogoService;
        }

        /// <summary>
        /// Pesquisar Jogos de froma paginada
        /// </summary>
        /// <remarks>
        /// Não é possível encontrar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina"> Indica qual página está sendo consultada. Mínimo 1</param>
        /// <param name="quantidade"> Indica a quantidade de resistros por página. Mínimo 1 e máximo 50</param>
        /// <response code="200"> Retorna a lista de jogos</response>
        /// <response code="204"> Caso não haja jogos</response>   
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1, int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quantidade = 5)
        {
            var jogos = await _jogoService.Obter(pagina, quantidade);

            if (jogos.Count() == 0)
                return NoContent();

            return Ok(jogos);
        }

        /// <summary>
        /// Buscar um jogo pelo seu Código
        /// </summary>
        /// <param name="codigoJogo"> Código de jogo pretendido</param>
        /// <response code="200">Retorna o jogo filtrado</response>
        /// <response code="204">Caso não haja jogo com este id</response>   

        [HttpGet("{codigoJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid codigoJogo)
        {
            var jogo = await _jogoService.Obter(codigoJogo);

            if (jogo == null)
                return NoContent();

            return Ok(jogo);
        }

        /// <summary>
        /// Inserir um jogo no catálogo
        /// </summary>
        /// <param name="jogoInputModel">Dados do jogo a ser inserido</param>
        /// <response code="200">Cao o jogo seja inserido com sucesso</response>
        /// <response code="422">Caso já exista um jogo com mesmo nome para a mesma produtora</response>   
        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> InserirJogo([FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                var jogo = await _jogoService.Inserir(jogoInputModel);

                return Ok(jogo);
            }
            catch (JogoJaCadastradoException ex)
            {
                return UnprocessableEntity("Já existe um jogo com este nome para esta produtora");
            }
        }

        /// <summary>
        /// Atualizar opção de jogo no catálogo
        /// </summary>
        /// /// <param name="codigoJogo">Código do jogo a ser atualizado</param>
        /// <param name="jogoInputModel">Novos dados para atualizar o jogo indicado</param>
        /// <response code="200">Cao o jogo seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response>   
        [HttpPut("{codigoJogo:guid}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid codigoJogo, [FromBody] JogoInputModel jogoInputModel)
        {
            try
            {
                await _jogoService.Atualizar(codigoJogo, jogoInputModel);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Atualizar o preço de um jogo
        /// </summary>
        /// /// <param name="codigoJogo">Código do jogo a ser atualizado</param>
        /// <param name="preco">Novo preço do jogo</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Código</response>
        [HttpPatch("{codigoJogo:guid}/preco/{preco:double}")]
        public async Task<ActionResult> AtualizarJogo([FromRoute] Guid codigoJogo, [FromRoute] double preco)
        {
            try
            {
                await _jogoService.Atualizar(codigoJogo, preco);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }

        /// <summary>
        /// Excluir um jogo
        /// </summary>
        /// /// <param name="codigoJogo">Código do jogo a ser excluído</param>
        /// <response code="200">Cao o preço seja atualizado com sucesso</response>
        /// <response code="404">Caso não exista um jogo com este Id</response> 
        [HttpDelete("{codigoJogo:guid}")]
        public async Task<ActionResult> ApagarJogo([FromRoute] Guid codigoJogo)
        {
            try
            {
                await _jogoService.Remover(codigoJogo);

                return Ok();
            }
            catch (JogoNaoCadastradoException ex)
            {
                return NotFound("Não existe este jogo");
            }
        }
    }
}
