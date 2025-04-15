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
}; // Faltava fechar corretamente o objeto crud

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
        novaOrdem: "" // Adicionei a propriedade faltante
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            // Inicialização do Nestable
            $(document).ready(function () {
                $('#mdPlanCurso').on('shown.bs.modal', function () {
                    self.InitializeNestable();
                });
            });

            // Restante das inicializações
            var cargaHoraria = $("#cargaHoraria");
            cargaHoraria.mask('000', { reverse: false });

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

            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {
                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);
                        $this.themePluginIOS7Switch();
                    });
                });
            }

            // Validação de formulários
            var formid = $('form')[1].id;
            if (formid === "formEditCurso") {
                $("#formEditCurso").validate({ /* ... */ });
            }
            if (formid === "formCurso") {
                $("#formCurso").validate({ /* ... */ });
            }

        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            // Implementação existente
        },
        DeleteCurso: function (id) {
            // Implementação existente
        },
        EditCurso: function (id) {
            // Implementação existente
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
            $('#nestable').nestable('destroy');
            $('#nestable').nestable({
                maxDepth: 2,
                group: 1
            }).on('change', function (e) {
                vm.novaOrdem = JSON.stringify($('#nestable').nestable('serialize'));
            });
        },
        SalvarOrdem: function () {
            axios.post('/Curso/SalvarOrdem', {
                cursoId: this.editDto.Id,
                novaOrdem: this.novaOrdem
            })
                .then(response => {
                    if (response.data.success) {
                        toastr.success('Ordem atualizada com sucesso!');
                        $('#mdPlanCurso').modal('hide');
                    }
                })
                .catch(error => {
                    toastr.error('Erro ao salvar ordem');
                });
        }
    }
});