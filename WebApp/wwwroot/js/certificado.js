var vm = new Vue({
    el: "#vCertificado",
    data: {
        loading: false,
        editDto: {
            Id: "", TipoCurso: "", Curso: "",
            ImagemFrente: "", HtmlFrente: "",
            ImagemVerso: "", HtmlVerso: "",
            NomeImagemFrente: "", NomeImagemVerso: "",
            Fomento: "", Url: "", Nome: "",
            Status: true
        }
    },
    mounted: function () {
        $('.select2').select2({ theme: "bootstrap" });

        $('[data-plugin-ios-switch]').each(function () {
            var $this = $(this);
            $this.themePluginIOS7Switch();
        });

        $('#formCertificado').validate({
            highlight: function (label) {
                $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
            },
            success: function (label) {
                $(label).closest('.form-group').removeClass('has-error');
                label.remove();
            },
            errorPlacement: function (error, element) {
                var placement = element.closest('.input-group');
                if (!placement.get(0)) placement = element;
                if (error.text() !== '') placement.after(error);
            }
        });
    },
    methods: {
        //ShowLoad: function (flag, el) {
        //    this.loading = flag;
        //    $("#" + el).loadingOverlay({ "startShowing": flag });
        //    if (!flag) {
        //        $("#" + el).removeClass("loading-overlay-showing");
        //    } else {
        //        $("#" + el).addClass("loading-overlay-showing");
        //    }
        //},
        DeleteCertificado: function (id) {
            var url = "Certificado/Delete/" + id;
            $("#deleteCertificadoHref").prop("href", url);
        },
        EditCertificado: function (id) {
            var self = this;
            self.editDto = {
                Id: "",
                Fomento: "",
                Url: "",
                Nome: "",
                Status: true
            };

            axios.get("Certificado/GetCertificadoById/?id=" + id).then(result => {
                self.$nextTick(() => {
                    self.editDto = {
                        Id: result.data.id,
                        Fomento: result.data.fomento,
                        Status: result.data.status,
                        Url: result.data.url && result.data.url.includes("\\Certificados")
                            ? "\\Certificados" + result.data.url.split("\\Certificados")[1]
                            : null,
                        Nome: result.data.nome,
                    };
                });

                if (result.data.listFomentos && result.data.listFomentos.length > 0) {
                    var items = '<option value="">Selecionar o Fomento</option>';
                    $("#ddlFomento").empty();
                    $.each(result.data.listFomentos,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlFomento").html(items);
                } else {
                    Site.Notification("Fomento", "Fomentos não encontrados.", "warning", 1);
                }

            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
            });
        },
        Certificado: function (id) {
            var self = this;
            return axios.get("/Certificado/GetCertificadoById/?id=" + id).then(result => {
                var data = result.data;
                self.editDto.Id = data.id;
                self.editDto.TipoCurso = data.tipoCurso;
                self.editDto.Curso = data.curso;
                self.editDto.HtmlFrente = data.htmlFrente;
                self.editDto.HtmlVerso = data.htmlVerso;
                self.editDto.Status = data.status;
                self.editDto.ImagemFrente = "/Certificados/" + data.imagemFrente;
                self.editDto.ImagemVerso = data.imagemVerso ? "/Certificados/" + data.imagemVerso : null;

                if (self.editDto.ImagemFrente) {
                    applyBackgroundImage('summernoteFrente', self.editDto.ImagemFrente);
                }
                if (self.editDto.ImagemVerso) {
                    applyBackgroundImage('summernoteVerso', self.editDto.ImagemVerso);
                }
            }).catch(error => {
                alert("Erro ao carregar certificado: " + error.message);
            });
        },
        ValidateFileType: function () {
            var fileName = document.getElementById("arquivo").value;
            var idxDot = fileName.lastIndexOf(".") + 1;
            var extFile = fileName.substr(idxDot, fileName.length).toLowerCase();
            if (extFile === "pdf") {
                //TO DO
            } else {
                Site.Notification("Erro ao realizar Upload", "Somente arquivos PDF são permitidos.", "error", 2);

            }
        }
    }
});

function applyBackgroundImage(editorId, imageUrl) {
    $('#' + editorId).next('.note-editor').find('.note-editable').css({
        'background-image': 'url(' + imageUrl + ')',
        'background-size': 'contain',
        'background-repeat': 'no-repeat',
        'background-position': 'center center'
    });
}

$(document).ready(function () {
    var id = $('#certificadoId').val();

    if (id) {
        vm.Certificado(id);
    }

    $('#btnImprimirCertificado').on('click', function () {

        // Função para extrair os dados essenciais de cada lado do certificado
        function extractCertificateData(side) {
            var container = $('#mdCertificado #' + side);
            if (!container.length) return null;

            var data = {
                imagem: '',
                html: ''
            };

            // Pega a URL da imagem
            var img = container.find('img');
            if (img.length) {
                data.imagem = img.attr('src') || '';
            }

            // Pega o HTML overlay (conteúdo dinâmico)
            var htmlOverlay = container.find('[v-html]');
            if (htmlOverlay.length) {
                data.html = htmlOverlay.html() || '';
            } else {
                // Fallback: pega diretamente do Vue.js
                if (side === 'frente' && vm.editDto.HtmlFrente) {
                    data.html = vm.editDto.HtmlFrente;
                } else if (side === 'verso' && vm.editDto.HtmlVerso) {
                    data.html = vm.editDto.HtmlVerso;
                }
            }

            return data;
        }

        // Extrai os dados de ambos os lados
        var dadosFrente = extractCertificateData('frente');
        var dadosVerso = extractCertificateData('verso');

        // Debug
        console.log('Dados Frente:', dadosFrente);
        console.log('Dados Verso:', dadosVerso);

        // Verifica se tem conteúdo para imprimir
        if (!dadosFrente && !dadosVerso) {
            alert("Erro ao localizar o conteúdo do certificado para impressão.");
            return;
        }

        // Função para criar o HTML de uma página de certificado
        function createCertificatePage(dados, pageId) {
            if (!dados || (!dados.imagem && !dados.html)) return '';

            return `
            <div class="certificate-page" id="${pageId}">
                <div class="certificate-container">
                    ${dados.imagem ? `<img src="${dados.imagem}" class="certificate-bg" alt="Certificado" />` : ''}
                    ${dados.html ? `<div class="certificate-overlay">${dados.html}</div>` : ''}
                </div>
            </div>
        `;
        }

        // Cria as páginas
        var paginaFrente = createCertificatePage(dadosFrente, 'pagina-frente');
        var paginaVerso = createCertificatePage(dadosVerso, 'pagina-verso');

        // Conta quantas páginas serão criadas
        var totalPaginas = (paginaFrente ? 1 : 0) + (paginaVerso ? 1 : 0);
        console.log('Total de páginas que serão criadas:', totalPaginas);

        if (totalPaginas === 0) {
            alert("Nenhum conteúdo encontrado para impressão.");
            return;
        }

        // Monta o HTML final para impressão
        var printWindow = window.open('', '_blank', 'width=1200,height=900');
        printWindow.document.write(`
        <html>
        <head>
            <title>Impressao-Certificado</title>
            <style>
                @page {
                    size: A4 landscape;
                    margin: 0;
                }
                
                * {
                    margin: 0;
                    padding: 0;
                    box-sizing: border-box;
                }
                
                body { 
                    background: white; 
                    font-family: Arial, sans-serif;
                    -webkit-print-color-adjust: exact;
                    print-color-adjust: exact;
                }
                
                .certificate-page {
                    width: 100vw;
                    height: 100vh;
                    page-break-after: always;
                    page-break-inside: avoid;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    position: relative;
                }
                
                .certificate-page:last-child {
                    page-break-after: avoid;
                }
                
                .certificate-container {
                    position: relative;
                    width: 1140px;
                    height: 813px;
                    max-width: 90vw;
                    max-height: 90vh;
                }
                
                .certificate-bg {
                    width: 100%;
                    height: 100%;
                    object-fit: contain;
                    position: absolute;
                    top: 0;
                    left: 0;
                    z-index: 1;
                }
                
                .certificate-overlay {
                    position: absolute;
                    top: 0;
                    left: 0;
                    width: 100%;
                    height: 100%;
                    z-index: 2;
                    pointer-events: none;
                    color: black;
                }
                
                /* Remove qualquer elemento que possa causar quebra de página */
                .certificate-overlay * {
                    page-break-before: avoid !important;
                    page-break-after: avoid !important;
                    page-break-inside: avoid !important;
                }
            </style>
        </head>
        <body>
            ${paginaFrente}
            ${paginaVerso}
        </body>
        </html>
    `);

        printWindow.document.close();
        printWindow.focus();

        // Aguarda carregamento das imagens
        setTimeout(function () {
            printWindow.print();
            setTimeout(function () {
                printWindow.close();
            }, 100);
        }, 1500);
    });
});

var crud = {
    CertificadoModal: function (id) {
        $('input[name="certificadoId"]').val(id);
        $('#mdCertificado').modal('show');
        vm.Certificado(id);
    },
    EditModal: function (id) {
        $('input[name="editCertificadoId"]').val(id);
        $('#mdEditCertificado').modal('show');
        vm.EditCertificado(id);
    },
    DeleteModal: function (id) {
        $('input[name="deleteCertificadoId"]').val(id);
        $('#mdDeleteCertificado').modal('show');
        vm.DeleteCertificado(id);
    }
};