var crud = {
    DeleteModal: function (id) {
        $('input[name="deleteModuloEadId"]').attr('value', id);
        $('#mdDeleteModuloEad').modal('show');
        vm.DeleteModuloEad(id)
    },
    EditModal: function (id) {
        $('input[name="editModuloEadId"]').attr('value', id);
        $('#mdEditModuloEad').modal('show');
        vm.EditModuloEad(id)
    },
    PlanModal: function (id) {
        $('input[name="editModuloEadId"]').val(id);
        $('#mdPlanModuloEad').modal('show');
        vm.PlanModuloEad(id);
    }
};

var vm = new Vue({
    el: "#vModuloEad ",
    data: {
        loading: false,
        editDto: { Id: "", Titulo: "", Descricao: "", Status: true }
    },
    mounted: function () {
        var self = this;
        (function ($) {
            'use strict';

            //skin checkbox
            if (typeof Switch !== 'undefined' && $.isFunction(Switch)) {

                $(function () {
                    $('[data-plugin-ios-switch]').each(function () {
                        var $this = $(this);

                        $this.themePluginIOS7Switch();
                    });
                });
            }

            var formid = $('form')[1].id;

            if (formid === "formEditModuloEad") {

                $("#formEditModuloEad ").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }

            if (formid === "formModuloEad") {

                //skin select
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

                /*
                 * When you change the value the select via select2, it triggers
                 * a 'change' event, but the jquery validation plugin
                 * only re-validates on 'blur'*/

                $select.on('change', function () {
                    $(this).trigger('blur');
                });

                //Açao de seleçao de valor na combo primaria para preencher a combo secundára
                $("#ddlTipoCurso").change(function () {
                    var tipoCursoId = $("#ddlTipoCurso").val();

                    var url = "../Curso/GetCursosAllByTipoCursoId";

                    var ddlSource = "#ddlCurso";

                    $.getJSON(url,
                        { id: tipoCursoId },
                        function (data) {
                            if (data.length > 0) {
                                var items = '<option value="">Selecionar Curso</option>';
                                $("#ddlCurso").empty;
                                $.each(data,
                                    function (i, row) {
                                        items += "<option value='" + row.value + "'>" + row.text + "</option>";
                                    });
                                $("#ddlCurso").html(items);
                            }
                            else {
                                new PNotify({
                                    title: 'Curso',
                                    text: 'Cursos não encontrados.',
                                    type: 'warning'
                                });
                            }
                        });
                });

                $("#formModuloEad").validate({
                    highlight: function (label) {
                        $(label).closest('.form-group').removeClass('has-success').addClass('has-error');
                    },
                    success: function (label) {
                        $(label).closest('.form-group').removeClass('has-error');
                        label.remove();
                    },
                    errorPlacement: function (error, element) {
                        var placement = element.closest('.input-group');
                        if (!placement.get(0)) {
                            placement = element;
                        }
                        if (error.text() !== '') {
                            placement.after(error);
                        }
                    }
                });
            }
        }).apply(this, [jQuery]);
    },
    methods: {
        ShowLoad: function (flag, el) {
            var self = this;

            self.isLoading = flag;
            $("#" + el).loadingOverlay({
                "startShowing": flag
            });
            self.loading = flag;

            if (!flag) {
                self.isLoading = flag;
                $("#" + el).removeClass("loading-overlay-showing");
                self.loading = flag;
            } else {
                self.isLoading = flag;
                $("#" + el).addClass("loading-overlay-showing");
                self.loading = flag;
            }
        },
        DeleteModuloEad: function (id) {
            var url = "ModuloEad/Delete/" + id;
            $("#deleteModuloEadHref").prop("href", url);
        },
        EditModuloEad: function (id) {
            var self = this;

            axios.get("ModuloEad/GetModuloEadById/?id=" + id).then(result => {

                self.editDto.Id = result.data.id;
                self.editDto.Titulo = result.data.titulo;
                self.editDto.Descricao = result.data.descricao;
                self.editDto.Status = result.data.status;

            }).catch(error => {
                Site.Notification("Erro ao buscar e analisar dados", error.message, "error", 1);
            });
        },
        PlanModuloEad: function (id) {
            this.editDto.Id = id;
            this.LoadEstruturaModuloEad(id);
        },
        LoadEstruturaModuloEad: function (id) {
            var self = this;
            $('#nestable-container').html('');
            $('.loading-overlay').show();

            $.ajax({
                url: '/ModuloEad/CarregarEstrutura',
                type: 'GET',
                data: { moduloId: id },
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

            // Adicionado índice no loop
            items.forEach(async (aula, index) => {
                try {
                    const response = await axios.get("Aula/GetAulaById/?id=" + aula.id.replace('aula_', ''));
                    const aulaData = {
                        Id: response.data.id,
                        Titulo: response.data.titulo,
                        ProfessorId: response.data.professorId,
                        Video: response.data.video,
                        NomeMaterial: response.data.nomeMaterial,
                        Material: response.data.material,
                        Descricao: response.data.descricao,
                        Status: response.data.status,
                        Ordem: index + 1
                    };

                    requests.push(
                        axios.post(`/Aula/Order/${aulaData.Id}`, aulaData, {
                            headers: {
                                'Content-Type': 'application/json',
                                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
                            }
                        })
                    );

                } catch (error) {
                    console.error('Erro ao buscar aula:', error);
                }
            });

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