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
            }).catch(error => {
                console.error('Erro ao carregar dados:', error);
            });
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
                url: '/Curso/CarregarEstrutura',
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
            }

            $('#nestable').nestable({
                maxDepth: 2,
                group: 1
            }).on('change', function (e) {
                const serialized = $(this).nestable('serialize');
                self.novaOrdem = JSON.stringify(serialized);
                self.AtualizarNumeracaoVisual(serialized); // Atualização imediata
                self.AtualizarOrdem(serialized); // Persistência no banco
            });

            // Atualizar posições iniciais
            this.AtualizarNumeracaoVisual($('#nestable').nestable('serialize'));
        },
        AtualizarOrdem: async function (items) {
            const requests = [];

            for (const modulo of items) {
                if (modulo.children) {
                    for (const [index, aula] of modulo.children.entries()) {
                        try {
                            const response = await axios.get("Aula/GetAulaById/?id=" + aula.id.replace('aula_', ''));
                            const aulaData = {
                                ...response.data,
                                Ordem: index + 1
                            };

                            requests.push(
                                axios.put("../../Aula/?id=" + aulaData.id, aulaData, {
                                    headers: {
                                        'Content-Type': 'application/json',
                                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                                    }
                                })
                            );
                        } catch (error) {
                            console.error('Erro ao buscar aula:', error);
                        }
                    }
                }
            }

            try {
                const responses = await Promise.all(requests);
                const allSuccess = responses.every(r => r.data.success);

                if (allSuccess) {
                    toastr.success('Ordem atualizada com sucesso!');
                    this.AtualizarNumeracaoVisual(items);
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
                    modulo.children.forEach((aula, aulaIndex) => {
                        const aulaNumber = aulaIndex + 1;
                        $(`[data-id="${aula.id}"] .position-badge`).text(`${moduloNumber}.${aulaNumber}`);
                    });
                }
            });
        },
        AtualizarNumeracaoVisual: function (items) {
            // Forçar atualização do DOM
            this.$nextTick(() => {
                items.forEach((modulo, modIndex) => {
                    const moduloNumber = modIndex + 1;
                    $(`[data-id="${modulo.id}"] .position-badge`).text(moduloNumber);

                    if (modulo.children) {
                        modulo.children.forEach((aula, aulaIndex) => {
                            const aulaNumber = aulaIndex + 1;
                            $(`[data-id="${aula.id}"] .position-badge`).text(`${moduloNumber}.${aulaNumber}`);
                        });
                    }
                });
            });
        }
    }
});