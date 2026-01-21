using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using VeTLink.DTOs.Responses;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace VeTLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchivosController(IWebHostEnvironment env, IConfiguration _config) : ControllerBase
    {
        const string r2AccessKeyId = "5617b0f960a5167e000d083710d07ae5";
        const string r2SecretAccessKey = "83159d7235eb12cb273f51b563eb58c9248feb25d3eb49eeb9082763e691b31c";
        const string r2BucketName = "vetlink";
        const string r2ServiceUrl = "https://c5744f7e8eac238dbc9d3ddb611039b7.r2.cloudflarestorage.com";

        private AmazonS3Client GetR2Client()
        {
            var credentials = new BasicAWSCredentials(r2AccessKeyId, r2SecretAccessKey);
            var config = new AmazonS3Config
            {
                ServiceURL = r2ServiceUrl,
                ForcePathStyle = true
            };
            return new AmazonS3Client(credentials, config);
        }

        private readonly IWebHostEnvironment _env = env;
        private readonly IConfiguration config = _config;

        /// <summary>
        /// Sube un archivo al bucket de R2 de Cloudflare
        /// </summary>
        /// <param name="file">Archivo a subir</param>
        /// <param name="key">Nombre/ruta del archivo en el bucket (opcional, si no se proporciona usa el nombre original)</param>
        /// <returns>Información sobre el archivo subido</returns>
        [HttpPost("put-file")]
        [RequestSizeLimit(2147483648)] // 2 GB
        [RequestFormLimits(MultipartBodyLengthLimit = 2147483648)]
        public async Task<IActionResult> PutFile(IFormFile file, [FromForm] string? key = null)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No se proporcionó ningún archivo o el archivo está vacío" });
            }

            // Validar tamaño máximo (2 GB)
            const long maxFileSize = 2147483648; // 2 GB
            if (file.Length > maxFileSize)
            {
                return BadRequest(new
                {
                    error = "Archivo demasiado grande",
                    message = $"El tamaño máximo permitido es {maxFileSize / (1024 * 1024)} MB. Tu archivo tiene {file.Length / (1024 * 1024)} MB.",
                    maxSizeMB = maxFileSize / (1024 * 1024),
                    currentSizeMB = file.Length / (1024 * 1024)
                });
            }

            try
            {
                using var s3Client = GetR2Client();

                // Si no se proporciona un key, usa el nombre original del archivo
                var objectKey = string.IsNullOrWhiteSpace(key) ? file.FileName : key;

                using var stream = file.OpenReadStream();

                var putRequest = new PutObjectRequest
                {
                    BucketName = r2BucketName,
                    Key = objectKey,
                    InputStream = stream,
                    ContentType = file.ContentType,
                    // Estos parámetros son requeridos para R2 de Cloudflare
                    DisablePayloadSigning = true,
                    DisableDefaultChecksumValidation = true
                };

                var response = await s3Client.PutObjectAsync(putRequest);

                return Ok(new
                {
                    success = true,
                    message = "Archivo subido exitosamente",
                    data = new
                    {
                        key = objectKey,
                        bucket = r2BucketName,
                        etag = response.ETag,
                        versionId = response.VersionId,
                        size = file.Length,
                        sizeMB = Math.Round(file.Length / (1024.0 * 1024.0), 2),
                        contentType = file.ContentType,
                        uploadedAt = DateTime.UtcNow
                    }
                });
            }
            catch (AmazonS3Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error al subir el archivo a R2",
                    message = ex.Message,
                    errorCode = ex.ErrorCode,
                    statusCode = ex.StatusCode
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Error inesperado al subir el archivo",
                    message = ex.Message
                });
            }
        }

        //[HttpPost("SubirImgCloudflare")]
        //[EndpointSummary("Sube un archivo de imagen con optimizacion automatica al repositorio de Cloudflare Images.")]
        //public async Task<ActionResult<RespuestaObjetoDTO>> SubirImgCloudflare([FromForm] FileUpload fileUpload)
        //{
        //    var respuesta = new RespuestaObjetoDTO();
        //    var archivo = fileUpload.Archivo;

        //    if (archivo == null || archivo.Length == 0)
        //    {
        //        respuesta.Message.Add("No se proporcionó ningún archivo.");
        //        return BadRequest(respuesta);
        //    }

        //    try
        //    {
        //        // Datos de Cloudflare desde configuración (appsettings.json)
        //        string accountId = config["Cloudflare:AccountId"]!;
        //        string apiToken = config["Cloudflare:ApiToken"]!;
        //        string email = config["Cloudflare:Email"]!;
        //        string baseUrl = "https://api.cloudflare.com";

        //        // Configura el cliente con un CookieContainer para gestionar las cookies automáticamente
        //        var options = new RestClientOptions(baseUrl)
        //        {
        //            MaxTimeout = -1,
        //            CookieContainer = new System.Net.CookieContainer()
        //        };
        //        var client = new RestClient(options);

        //        var request = new RestRequest($"/client/v4/accounts/{accountId}/images/v1", Method.Post);
        //        request.AddHeader("X-Auth-Email", email);
        //        request.AddHeader("Authorization", $"Bearer {apiToken}");
        //        request.AlwaysMultipartFormData = true;
        //        request.AddParameter("requireSignedURLs", "false");

        //        using (var stream = archivo.OpenReadStream())
        //        {
        //            var bytes = new byte[archivo.Length];
        //            await stream.ReadExactlyAsync(bytes, 0, (int)archivo.Length);
        //            request.AddFile("file", bytes, archivo.FileName, archivo.ContentType);
        //        }

        //        var response = await client.ExecuteAsync(request);

        //        if (!response.IsSuccessful)
        //        {
        //            // Incluir más detalles del error para facilitar la depuración
        //            string errorMessage = $"Error al subir el archivo: {response.StatusCode}. Contenido: {response.Content}. Mensaje de error: {response.ErrorMessage}";
        //            respuesta.Message.Add(errorMessage);
        //            return StatusCode((int)(response.StatusCode == 0 ? System.Net.HttpStatusCode.InternalServerError : response.StatusCode), respuesta);
        //        }

        //        var CloudResponse = JsonSerializer.Deserialize<CloudFlareResponseDTO>(
        //            response.Content!,
        //            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        //        );

        //        respuesta.Status = true;
        //        respuesta.Message.Add("Archivo subido correctamente.");
        //        respuesta.Response = CloudResponse!.result.variants;

        //        return Ok(respuesta);
        //    }
        //    catch (Exception ex)
        //    {
        //        respuesta.Message.Add($"Error interno: {ex.Message}");
        //        return StatusCode(500, respuesta);
        //    }
        //}

        //[HttpGet("LeerArchivosCloudflare/{*rutaArchivo}")]
        //[EndpointSummary("Lee o descarga un archivo específico desde Cloudflare R2.")]
        //public async Task<IActionResult> LeerArchivosCloudflare(string rutaArchivo)
        //{
        //    try
        //    {
        //        // Ejemplo de rutaArchivo: "convenciones/archivo.pdf"
        //        string bucketUrl = config["Cloudflare:R2BucketUrl"]!;
        //        string authorization = config["Cloudflare:R2Authorization"]!;

        //        var options = new RestClientOptions(bucketUrl)
        //        {
        //            MaxTimeout = -1,
        //        };

        //        var client = new RestClient(options);
        //        var request = new RestRequest($"/{rutaArchivo}", Method.Get);

        //        request.AddHeader("X-Amz-Content-Sha256", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        //        request.AddHeader("X-Amz-Date", DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ"));
        //        request.AddHeader("Authorization", authorization);

        //        var response = await client.ExecuteAsync(request);

        //        if (!response.IsSuccessful)
        //        {
        //            return StatusCode((int)response.StatusCode, new
        //            {
        //                Status = false,
        //                Message = $"Error al leer el archivo: {response.StatusCode}",
        //                Detalle = response.Content
        //            });
        //        }

        //        // Detectar tipo de archivo
        //        var contentType = response.ContentType ?? "application/octet-stream";
        //        var fileName = Path.GetFileName(rutaArchivo);

        //        // Retornar el archivo como FileResult
        //        return File(response.RawBytes!, contentType, fileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        //_logger.LogError(ex, "Error al leer archivo desde Cloudflare R2");
        //        return StatusCode(500, new
        //        {
        //            Status = false,
        //            Message = "Error interno al obtener el archivo",
        //            Detalle = ex.Message
        //        });
        //    }
        //}

        [HttpGet("TestConexionR2")]
        [EndpointSummary("Prueba la conexión con Cloudflare R2.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> TestConexionR2()
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {

                var s3Config = new AmazonS3Config
                {
                    ServiceURL = r2ServiceUrl,
                    ForcePathStyle = true
                };

                var credentials = new BasicAWSCredentials(r2AccessKeyId, r2SecretAccessKey);
                using var s3Client = new AmazonS3Client(credentials, s3Config);

                // Intentar listar objetos del bucket
                var listRequest = new ListObjectsV2Request
                {
                    BucketName = r2BucketName,
                    MaxKeys = 1
                };

                var listResponse = await s3Client.ListObjectsV2Async(listRequest);

                respuesta.Status = true;
                respuesta.Message.Add("Conexión exitosa con Cloudflare R2.");
                respuesta.Response = new
                {
                    BucketName = r2BucketName,
                    ServiceUrl = r2ServiceUrl,
                    ObjectCount = listResponse.KeyCount,
                    Success = true
                };

                return Ok(respuesta);
            }
            catch (AmazonS3Exception s3Ex)
            {
                respuesta.Message.Add($"Error de conexión R2: {s3Ex.Message} - {s3Ex.ErrorCode}");
                return StatusCode(500, respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Message.Add($"Error: {ex.Message}");
                return StatusCode(500, respuesta);
            }
        }
        private string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();

            return extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".zip" => "application/zip",
                _ => "application/octet-stream",
            };
        }

        [HttpGet("ListarArchivosCloudflare")]
        [EndpointSummary("Lista los archivos en una carpeta específica de Cloudflare R2.")]
        public async Task<ActionResult<RespuestaObjetoDTO>> ListarArchivosCloudflare(
            [FromQuery] string? carpeta = null,
            [FromQuery] int maxResultados = 100)
        {
            var respuesta = new RespuestaObjetoDTO
            {
                Message = []
            };

            try
            {
                using var s3Client = GetR2Client();

                var listRequest = new ListObjectsV2Request
                {
                    BucketName = r2BucketName,
                    MaxKeys = maxResultados
                };

                // Si se especifica una carpeta, agregar prefijo
                if (!string.IsNullOrWhiteSpace(carpeta))
                {
                    listRequest.Prefix = carpeta.TrimEnd('/') + "/";
                }

                var listResponse = await s3Client.ListObjectsV2Async(listRequest);

                var archivos = listResponse.S3Objects.Select(obj => new
                {
                    Key = obj.Key,
                    FileName = Path.GetFileName(obj.Key),
                    Size = obj.Size,
                    LastModified = obj.LastModified,
                    ETag = obj.ETag,
                    Url = $"{r2ServiceUrl}/{r2BucketName}/{obj.Key}"
                }).ToList();

                respuesta.Status = true;
                respuesta.Message.Add($"Se encontraron {archivos.Count} archivos.");
                respuesta.Response = new
                {
                    Carpeta = carpeta ?? "raíz",
                    TotalArchivos = archivos.Count,
                    Archivos = archivos
                };

                return Ok(respuesta);
            }
            catch (AmazonS3Exception s3Ex)
            {
                respuesta.Message.Add($"Error de Cloudflare R2: {s3Ex.Message} - {s3Ex.ErrorCode}");
                return StatusCode(500, respuesta);
            }
            catch (Exception ex)
            {
                respuesta.Message.Add($"Error interno: {ex.Message}");
                return StatusCode(500, respuesta);
            }
        }
    }

    public class FileUpload
    {
        [Required]
        public IFormFile Archivo { get; set; }
    }
    public class CloudflareR2UploadRequest
    {
        [Required]
        [Display(Description = "El archivo a subir.")]
        public IFormFile Archivo { get; set; }

        [Display(Description = "La carpeta de destino dentro del bucket de Cloudflare R2. Puede ser nula o vacía para subir a la raíz del bucket.")]
        public string? Carpeta { get; set; } // Usamos string? para indicar que puede ser nulo
    }


}