using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using aztableCrud.Context;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Azure;


namespace aztableCrud.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatoController : ControllerBase
    {   
        private readonly string? _connectionString;
        private readonly string? _tableName;
        public ContatoController(IConfiguration configuracao){
            _connectionString = configuracao.GetValue<string>("SAConnectionString");
            _tableName = configuracao.GetValue<string>("AzureTableName");
        }
        private TableClient GetTableClient(){
            var serviceClient = new TableServiceClient(_connectionString); //inicia o serviço
            var tableClient = serviceClient.GetTableClient(_tableName); //seleciona uma tabela do table storage
            tableClient.CreateIfNotExists();
            return (tableClient);
        }

        [HttpPost]
        public IActionResult Criar(Contato contato){
            var tableClient = GetTableClient();
            contato.RowKey = Guid.NewGuid().ToString();
            contato.PartitionKey = contato.RowKey;
            tableClient.UpsertEntity(contato);
            return Ok(contato);
        }
        [HttpPut("{id}")]
        public IActionResult Atualizar(string id, Contato contato){
            var tableClient = GetTableClient();
            var contatoTable = tableClient.GetEntity<Contato>(id, id).Value;

            contatoTable.Nome = contato.Nome;
            contatoTable.Telefone = contato.Telefone;
            contatoTable.Email = contato.Email;
            tableClient.UpsertEntity(contatoTable);
            return Ok();
        }
        [HttpGet]
        public IActionResult ObterContatos(){
            var tabela = GetTableClient();
           var contatos = tabela.Query<Contato>().ToList();
            return Ok(contatos);
        }
        [HttpGet("{nome}")]
        public IActionResult ObterNomes(string nome){
        var tabela = GetTableClient();
        var contatos = tabela.Query<Contato>(x => x.Nome == nome).ToList();
        return Ok(contatos);
        }
        [HttpDelete]
        public IActionResult Deletar(string id){
            var tableClient = GetTableClient();
            tableClient.DeleteEntity(id, id);
            return NoContent();
        }
    }
}