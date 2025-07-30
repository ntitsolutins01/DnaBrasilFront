var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteCursoId"]').val(id);
        $('#mdDeleteCurso').modal('show');
        vm.DeleteCurso(id);
    },
    EditModal: function (id) {
        $('input[name="editCursoId"]').val(id);
        $('#mdEditCurso').modal('show');
        vm.EditCurso(id);
    },
    PlanModal: function (id) {
        $('input[name="editCursoId"]').val(id);
        $('#mdPlanCurso').modal('show');
        vm.PlanCurso(id);
    }
};

var vm = new Vue({
    el: "#vCurso",
    data: {
        loading: false,
        editDto: {
            Id: "",
            Titulo: "",
            Descricao: "",
            CargaHoraria: "",
            Status: true,
            Imagem: "",
            NomeImagem: ""
        },
        novaOrdem: ""
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // Inicialização de componentes
            var cargaHoraria = $("#cargaHoraria");
            cargaHoraria.mask('000', { reverse: false });

            // Inicialização do Select2
            var $select = $(".select2").select2({
                allowClear: true
            });

            $(".select2").each(function () {
                var $this = $(this),
                    opts = {};

                var pluginOptions = $this.data('plugin-options');
                if (pluginOptions)
                    opts = pluginOptions;

                $this.themePluginSelect2(opts);
            });

            $select.on('change', function () {
                $(this).trigger('blur');
            });

            // Inicialização do Switch
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {
                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);
                        $this.themePluginIOS7Switch();
                    });
                });
            }

        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            this.loading = flag;
            $("#" + el).loadingOverlay({ "startShowing": flag });
            if (!flag) {
                $("#" + el).removeClass("loading-overlay-showing");
            } else {
                $("#" + el).addClass("loading-overlay-showing");
            }
        },
        DeleteCurso: function (id) {
            var url = "Curso/Delete/" + id;
            $("#deleteCursoHref").prop("href", url);
        },
        EditCurso: function (id) {
            var self = this;
            self.editDto = { Id: "", Titulo: "", Descricao: "", CargaHoraria: "", Status: true, Imagem: "", NomeImagem: "" };

            axios.get("Curso/GetCursoById/?id=" + id).then(result => {
                self.$nextTick(() => {
                    self.editDto = {
                        Id: result.data.id,
                        Titulo: result.data.titulo,
                        Descricao: result.data.descricao,
                        CargaHoraria: result.data.cargaHoraria,
                        Status: result.data.status,
                        Imagem: result.data.imagem && result.data.imagem.includes("\\Cursos")
                            ? "\\Cursos" + result.data.imagem.split("\\Cursos")[1]
                            : null,
                        NomeImagem: result.data.nomeImagem
                    };
                });

                if (result.data.listCoordenadores && result.data.listCoordenadores.length > 0) {
                    var items = '<option value="">Selecionar o Coordenador</option>';
                    $("#ddlCoordenador").empty();
                    $.each(result.data.listCoordenadores,
                        function (i, row) {
                            if (row.selected) {
                                items += "<option selected value='" + row.value + "'>" + row.text + "</option>";
                            } else {
                                items += "<option value='" + row.value + "'>" + row.text + "</option>";
                            }
                        });
                    $("#ddlCoordenador").html(items);
                } else {
                    Site.Notification("Coordenador", "Coordenadores não encontrados.", "warning", 1);
                }
            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
            });
        },
        ValidateFileType: function() {
            var fileName = document.getElementById("arquivo").value;
            var idxDot = fileName.lastIndexOf(".") + 1;
            var extFile = fileName.substr(idxDot, fileName.length).toLowerCase();
            if (extFile === "jpg" || extFile === "jpeg" || extFile === "png") {
                //TO DO
            } else {
                Site.Notification("Erro ao realizar Upload", "Somente arquivos JPG/JPEG e PNG são permitidos.", "error", 2);
                
            }   
        },
        PlanCurso: function (id) {
            this.editDto.Id = id;
            this.LoadEstruturaCurso(id);
        },
        LoadEstruturaCurso: function (id) {
            var self = this;
            $('#nestable-container').html('');
            $('.loading-overlay').show();

            $.ajax({
                url: 'Curso/CarregarEstrutura',
                type: 'GET',
                data: { cursoId: id },
                success: function (response) {
                    self.$nextTick(() => {
                        $('#nestable-container').html(response);
                        self.InitializeNestable();
                    });
                },
                complete: function () {
                    $('.loading-overlay').hide();
                }
            });
        },
        InitializeNestable: function () {
            var self = this;

            if ($('#nestable').data('nestable')) {
                $('#nestable').nestable('destroy');
                $('#nestable').off('change');
            }

            $('#nestable').nestable({
                maxDepth: 1,
                group: 1
            }).on('change', function (e) {
                const serialized = $(this).nestable('serialize');
                self.novaOrdem = JSON.stringify(serialized);
                self.AtualizarNumeracaoVisual(serialized);
                self.AtualizarOrdem(serialized);
            });

            this.AtualizarNumeracaoVisual($('#nestable').nestable('serialize'));
        },
        AtualizarOrdem: async function (items) {
            const requests = [];

            items.forEach(async (moduloEad, index) => {
                try {
                    const response = await axios.get("ModuloEad/GetModuloEadById/?id=" + moduloEad.id.replace('moduloEad_', ''));
                    const moduloEadData = {
                        Id: response.data.id,
                        Titulo: response.data.titulo,
                        CursoId: response.data.cursoId,
                        Descricao: response.data.descricao,
                        Status: response.data.status,
                        Ordem: index + 1
                    };

                    requests.push(
                        axios.post(`/ModuloEad/Order/${moduloEadData.Id}`, moduloEadData, {
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                            }
                        })
                    );

                } catch (error) {
                    console.error('Erro ao buscar módulo ead:', error);
                }
            });

            try {
                const responses = await Promise.all(requests);
                const allSuccess = responses.every(r => r.data.success);

                if (allSuccess) {
                    new PNotify({
                        title: 'Módulo Ead',
                        text: 'Ordem alterada com sucesso!',
                        type: 'success'
                    });
                }
            } catch (error) {
                console.error('Erro:', error);
                toastr.error('Erro ao atualizar ordem');
            }
        },
        AtualizarNumeracaoVisual: function (items) {
            items.forEach((modulo, modIndex) => {
                const moduloNumber = modIndex + 1;
                $(`[data-id="${modulo.id}"] .position-badge`).text(moduloNumber);

                if (modulo.children) {
                    modulo.children.forEach((moduloEad, moduloEadIndex) => {
                        const moduloEadNumber = moduloEadIndex + 1;
                        $(`[data-id="${moduloEad.id}"] .position-badge`).text(`${moduloNumber}.${moduloEadNumber}`);
                    });
                }
            });
        },
        AtualizarNumeracaoVisual: function (items) {
            this.$nextTick(() => {
                items.forEach((modulo, modIndex) => {
                    const moduloNumber = modIndex + 1;
                    $(`[data-id="${modulo.id}"] .position-badge`).text(moduloNumber);

                    if (modulo.children) {
                        modulo.children.forEach((moduloEad, moduloEadIndex) => {
                            const moduloEadNumber = moduloEadIndex + 1;
                            $(`[data-id="${moduloEad.id}"] .position-badge`).text(`${moduloNumber}.${moduloEadNumber}`);
                        });
                    }
                });
            });
        }
    }
});
