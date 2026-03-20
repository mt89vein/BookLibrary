import http from 'k6/http'
import { check } from 'k6'

export default function () {

  let res = http.get('http://localhost:50816/api/v1/books?bookId=0191654a-00dc-747f-90f9-ec0a0499b77e', {
    headers: { 
        'Content-Type': 'application/json',
        'Cookie': 'sid=%5B%7B%22Type%22%3A%22email%22%2C%22Value%22%3A%22my-email%40gmail.com%22%7D%2C%7B%22Type%22%3A%22sub%22%2C%22Value%22%3A%2201916536-3f02-700a-aba4-aad273e55058%22%7D%5D' },
  });

  check(res, { 'success': (r) => r.status === 200 })
}