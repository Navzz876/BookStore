$(document).ready(function () {
    loadDataTable();
});
var dataTable;
function loadDataTable() {
    dataTable = $('#companyTable').DataTable({
        "ajax": { url: '/Admin/Company/GetAll' },
        "columns": [
            { data: 'name' },
            { data: 'city' },
            { data: 'state' },
            { data: 'country' },
            { data: 'phone' },
            {
                data: 'id',
                "render": function (data) {
                    return `<a class=" link-primary text-primary" href="/Admin/Company/EditCompany?id=${data}"><i class="bi bi-pencil-square text-primary"></i> Edit </a>`
                }
            },
            {
                data: 'id',
                "render": function (data) {
                    return `<a onClick= Delete("/Admin/Company/DeleteCompany?id=${data}")> <i class="bi bi-trash-fill text-danger"></i> </a>`
                }
            }

        ]
    })
}
function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    Swal.fire(
                        data.message
                    )
                }
            })
        }
    })

}
